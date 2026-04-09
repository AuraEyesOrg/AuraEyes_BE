using Application.OrganisationScreenings.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ImgSharpColor = SixLabors.ImageSharp.Color;
using ImgSharpImage = SixLabors.ImageSharp.Image;

namespace Infrastructure.Services;

public sealed class OrganisationScreeningPdfService : IOrganisationScreeningPdfService
{
    private const string BrandBlue = "#0B4A8B";
    private const string BrandSky = "#1C7ED6";
    private const string Surface = "#F8FAFC";
    private const string Border = "#D0D5DD";
    private const string TextStrong = "#101828";
    private const string TextMuted = "#475467";
    private static readonly HttpClient ImageHttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(8)
    };

    static OrganisationScreeningPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateScreeningReportPdf(OrgScreeningReportPdfModel model)
    {
        var originalImageUrl = model.OriginalImageUrls.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        var originalImageData = TryDownloadImageData(originalImageUrl);
        var annotatedImageData = TryDownloadImageData(model.AnnotatedImageUrl)
            ?? TryBuildBoxedImage(originalImageData, model.LocalizationBoxes);
        var heatmapImageData = TryDownloadImageData(model.HeatmapImageUrl);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Element(c => ComposeHeader(c, model));

                page.Content().PaddingVertical(14).Column(column =>
                {
                    column.Spacing(12);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => ComposePatientInfo(c, model));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => ComposeSessionInfo(c, model));
                    });

                    column.Item().Element(c => ComposeHeroResult(c, model));

                    column.Item().Element(c => ComposeRetinalImagesSection(
                        c,
                        originalImageData,
                        annotatedImageData,
                        heatmapImageData,
                        originalImageUrl,
                        model.AnnotatedImageUrl,
                        model.HeatmapImageUrl));

                    column.Item().Element(c => ComposeAiFindingsTable(c, model));
                });

                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf();
    }

    private void ComposeHeader(IContainer container, OrgScreeningReportPdfModel model)
    {
        container.Border(1).BorderColor(Border).Padding(12).Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("AURA EYES").FontSize(22).SemiBold().FontColor(BrandBlue);
                    left.Item().Text("Retinal AI Screening Report")
                        .FontSize(12)
                        .SemiBold()
                        .FontColor(TextMuted);
                });

                row.RelativeItem().AlignRight().Column(right =>
                {
                    right.Spacing(2);
                    right.Item().Text("Healthcare Copy")
                        .FontSize(9)
                        .SemiBold()
                        .FontColor(BrandSky);
                    right.Item().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(9).FontColor(TextStrong));
                        text.Span("Organisation: ").SemiBold();
                        text.Span(model.OrganisationName);
                    });
                    right.Item().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(9).FontColor(TextMuted));
                        text.Span("Generated: ").SemiBold();
                        text.Span(DateTime.UtcNow.ToString("dd MMM yyyy HH:mm 'UTC'"));
                    });
                });
            });
            column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Border);
        });
    }

    private void ComposePatientInfo(IContainer container, OrgScreeningReportPdfModel model)
    {
        Card(container).Column(column =>
        {
            column.Spacing(4);
            column.Item().Text("Patient Information")
                .SemiBold()
                .FontSize(12)
                .FontColor(BrandBlue);
            column.Item().PaddingBottom(4).LineHorizontal(1).LineColor(Border);
            
            ComposeInfoRow(column, "Patient Name", model.PatientName);
            ComposeInfoRow(column, "Patient ID", model.PatientId.ToString());
        });
    }

    private void ComposeSessionInfo(IContainer container, OrgScreeningReportPdfModel model)
    {
        Card(container).Column(column =>
        {
            column.Spacing(4);
            column.Item().Text("Session Information")
                .SemiBold()
                .FontSize(12)
                .FontColor(BrandBlue);
            column.Item().PaddingBottom(4).LineHorizontal(1).LineColor(Border);
            
            ComposeInfoRow(column, "Screening ID", model.ScreeningId.ToString());
            ComposeInfoRow(column, "Model Version", string.IsNullOrWhiteSpace(model.ModelVersion) ? "N/A" : model.ModelVersion);
            ComposeInfoRow(column, "Session Created", model.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        });
    }

    private void ComposeHeroResult(IContainer container, OrgScreeningReportPdfModel model)
    {
        var riskColor = ResolveRiskColor(model.RiskLevel);

        container.BorderLeft(4).BorderColor(riskColor).Background(Surface).Padding(12).Column(column =>
        {
            column.Spacing(6);
            
            // Top Level Metrics
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Risk Level: ").SemiBold().FontSize(12).FontColor(TextStrong);
                    text.Span(string.IsNullOrWhiteSpace(model.RiskLevel) ? "N/A" : model.RiskLevel.ToUpperInvariant())
                        .SemiBold()
                        .FontSize(12)
                        .FontColor(riskColor);
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Highest Confidence: ").SemiBold().FontSize(12).FontColor(TextStrong);
                    text.Span(FormatPercentage(model.ConfidenceScore)).SemiBold().FontSize(12).FontColor(BrandBlue);
                });
            });

            column.Item().LineHorizontal(1).LineColor(Border);

            column.Item().PaddingVertical(2).Text(text =>
            {
                text.Span("AI Summary: ").SemiBold().FontColor(BrandBlue);
                text.Span(string.IsNullOrWhiteSpace(model.Summary) ? "No summary available." : " " + model.Summary).FontColor(TextStrong);
            });
            
            column.Item().Text(text =>
            {
                text.DefaultTextStyle(x => x.FontSize(8).FontColor(TextMuted));
                text.Span("Assessed At: ");
                text.Span(model.AssessedAt.HasValue ? model.AssessedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A");
            });
        });
    }

    private void ComposeAiFindingsTable(IContainer container, OrgScreeningReportPdfModel model)
    {
        container.Border(1).BorderColor(Border).Padding(10).Column(column =>
        {
            column.Spacing(6);
            column.Item().Text("AI Detailed Findings")
                .SemiBold()
                .FontSize(12)
                .FontColor(BrandBlue);

            if (model.AiFindingDetails.Count == 0)
            {
                column.Item().Background(Surface).Padding(10).Text(
                    "No structured finding percentages are available for this screening session.")
                    .FontColor(TextMuted);
                return;
            }

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);  // Rank Number
                    columns.RelativeColumn(3);   // Finding Name
                    columns.ConstantColumn(80);  // Confidence %
                    columns.ConstantColumn(110); // Status
                });

                table.Header(header =>
                {
                    header.Cell().Element(TableHeaderCell).AlignCenter().Text("#").FontColor(Colors.White).SemiBold();
                    header.Cell().Element(TableHeaderCell).Text("Finding").FontColor(Colors.White).SemiBold();
                    header.Cell().Element(TableHeaderCell).AlignRight().Text("Confidence").FontColor(Colors.White).SemiBold();
                    header.Cell().Element(TableHeaderCell).AlignCenter().Text("Status").FontColor(Colors.White).SemiBold();
                });

                foreach (var finding in model.AiFindingDetails.OrderBy(x => x.Rank))
                {
                    table.Cell().Element(TableBodyCell).AlignCenter().Text(finding.Rank.ToString()).FontColor(TextStrong);
                    table.Cell().Element(TableBodyCell).Text(finding.DiseaseName).FontColor(TextStrong);
                    table.Cell().Element(TableBodyCell).AlignRight().Text($"{finding.ConfidencePercentage:0.##}%").FontColor(TextStrong);
                    
                    var status = string.IsNullOrWhiteSpace(finding.Status) ? "Detected" : finding.Status.Replace("_", " ");
                    if (status.Length > 0)
                    {
                        status = char.ToUpperInvariant(status[0]) + status.Substring(1).ToLowerInvariant();
                    }
                    
                    table.Cell().Element(TableBodyCell).AlignCenter().Text(status).FontColor(TextMuted);
                }
            });
        });
    }

    private void ComposeRetinalImagesSection(
        IContainer container,
        byte[]? originalImageData,
        byte[]? annotatedImageData,
        byte[]? heatmapImageData,
        string? originalImageUrl,
        string? annotatedImageUrl,
        string? heatmapImageUrl)
    {
        container.Border(1).BorderColor(Border).Padding(10).Column(column =>
        {
            column.Spacing(8);
            column.Item().Text("Retinal Imagery")
                .SemiBold()
                .FontSize(12)
                .FontColor(BrandBlue);

            column.Item().Row(row =>
            {
                row.RelativeItem().Element(c => ComposeImageCard(
                    c,
                    "Original Scan",
                    originalImageData,
                    originalImageUrl));

                row.ConstantItem(8);

                row.RelativeItem().Element(c => ComposeImageCard(
                    c,
                    "AI Localization",
                    annotatedImageData,
                    annotatedImageData is null ? annotatedImageUrl : "generated"));

                row.ConstantItem(8);

                row.RelativeItem().Element(c => ComposeImageCard(
                    c,
                    "Attention Heatmap",
                    heatmapImageData,
                    heatmapImageUrl));
            });
        });
    }

    private void ComposeImageCard(IContainer container, string title, byte[]? imageData, string? sourceUrl)
    {
        container.Border(1).BorderColor(Border).Background(Surface).Padding(8).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text(title)
                .SemiBold()
                .FontSize(10)
                .FontColor(TextStrong)
                .AlignCenter();

            if (imageData is { Length: > 0 })
            {
                column.Item()
                    .Height(150)
                    .Border(1)
                    .BorderColor(Border)
                    .Padding(2)
                    .AlignCenter()
                    .AlignMiddle()
                    .Image(imageData)
                    .FitArea();
            }
            else
            {
                column.Item()
                    .Height(150)
                    .Border(1)
                    .BorderColor(Border)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("Image unavailable")
                    .FontSize(9)
                    .FontColor(TextMuted);
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Border);
            column.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text(
                    "This report is generated by AuraEyes AI to support medical evaluation and does not replace definitive clinical diagnosis.")
                    .FontSize(8)
                    .FontColor(TextMuted);

                row.ConstantItem(120).AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(TextMuted));
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        });
    }


    private static IContainer Card(IContainer container)
    {
        return container
            .Background(Surface)
            .Border(1)
            .BorderColor(Border)
            .Padding(10);
    }

    private static IContainer TableHeaderCell(IContainer container)
    {
        return container
            .Background(BrandBlue)
            .BorderBottom(1)
            .BorderColor(BrandBlue)
            .PaddingVertical(6)
            .PaddingHorizontal(8);
    }

    private static IContainer TableBodyCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Border)
            .PaddingVertical(6)
            .PaddingHorizontal(8);
    }

    private static void ComposeInfoRow(ColumnDescriptor column, string label, string value)
    {
        column.Item().Text(text =>
        {
            text.Span($"{label}: ").SemiBold().FontColor(TextStrong);
            text.Span(string.IsNullOrWhiteSpace(value) ? "N/A" : value).FontColor(TextMuted);
        });
    }

    private static string ResolveRiskColor(string? riskLevel)
    {
        return riskLevel?.Trim().ToLowerInvariant() switch
        {
            "critical" or "high" => "#B42318",
            "moderate" or "medium" => "#B54708",
            "low" => "#027A48",
            _ => BrandBlue
        };
    }

    private static string FormatPercentage(decimal? confidenceScore)
    {
        if (!confidenceScore.HasValue)
            return "N/A";

        var value = confidenceScore.Value <= 1m
            ? confidenceScore.Value * 100m
            : confidenceScore.Value;

        value = Math.Clamp(value, 0m, 100m);
        return $"{value:0.##}%";
    }

    private static byte[]? TryDownloadImageData(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        if (url.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase)) return TryDecodeDataImage(url);
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
        if (uri.Scheme is not ("http" or "https")) return null;

        try
        {
            var response = ImageHttpClient.GetAsync(uri).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (string.IsNullOrWhiteSpace(mediaType) || !mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return null;

            var data = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            return data.Length == 0 ? null : data;
        }
        catch
        {
            return null;
        }
    }

    private static byte[]? TryDecodeDataImage(string dataUrl)
    {
        var commaIndex = dataUrl.IndexOf(',');
        if (commaIndex <= 0 || commaIndex >= dataUrl.Length - 1) return null;

        var metadata = dataUrl[..commaIndex];
        var payload = dataUrl[(commaIndex + 1)..];

        if (!metadata.Contains(";base64", StringComparison.OrdinalIgnoreCase)) return null;

        try
        {
            return Convert.FromBase64String(payload);
        }
        catch
        {
            return null;
        }
    }

    private static byte[]? TryBuildBoxedImage(byte[]? originalImageData, IReadOnlyCollection<AiLocalizationBox> boxes)
    {
        if (originalImageData is not { Length: > 0 } || boxes.Count == 0)
            return null;

        try
        {
            using var image = ImgSharpImage.Load<Rgba32>(originalImageData);
            var lineThickness = Math.Max(2f, Math.Min(image.Width, image.Height) / 250f);
            var strokeColor = ImgSharpColor.FromRgba(59, 130, 246, 255);
            var fillColor = ImgSharpColor.FromRgba(59, 130, 246, 45);

            image.Mutate(ctx =>
            {
                foreach (var box in boxes)
                {
                    var x = Math.Clamp(box.X, 0, image.Width - 1);
                    var y = Math.Clamp(box.Y, 0, image.Height - 1);
                    var maxWidth = image.Width - x;
                    var maxHeight = image.Height - y;
                    var width = Math.Clamp(box.Width, 1, maxWidth);
                    var height = Math.Clamp(box.Height, 1, maxHeight);

                    var rectangle = new RectangleF(x, y, width, height);
                    ctx.Fill(fillColor, rectangle);
                    ctx.Draw(strokeColor, lineThickness, rectangle);
                }
            });

            using var output = new MemoryStream();
            image.Save(output, PngFormat.Instance);
            return output.ToArray();
        }
        catch
        {
            return null;
        }
    }
}