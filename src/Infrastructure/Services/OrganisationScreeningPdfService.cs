using Application.OrganisationScreenings.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services;

public sealed class OrganisationScreeningPdfService : IOrganisationScreeningPdfService
{
    private const string BrandBlue = "#0B4A8B";
    private const string BrandSky = "#1C7ED6";
    private const string Surface = "#F8FAFC";
    private const string Border = "#D0D5DD";
    private const string TextStrong = "#101828";
    private const string TextMuted = "#475467";

    static OrganisationScreeningPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateScreeningReportPdf(OrgScreeningReportPdfModel model)
    {
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

                    column.Item().Element(c => ComposeClinicalSummary(c, model));
                    column.Item().Element(c => ComposeAiFindingsTable(c, model));
                    column.Item().Element(c => ComposeTextSection(c, "AI Summary", model.Summary ?? "No summary available."));
                    column.Item().Element(c => ComposeTextSection(c, "Clinical Findings & Notes", model.Findings ?? "No findings available."));
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
            ComposeInfoRow(column, "Organisation", model.OrganisationName);
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
            ComposeInfoRow(column, "Images Analyzed", model.ImagesCount.ToString());
            ComposeInfoRow(column, "Session Created", model.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        });
    }

    private void ComposeClinicalSummary(IContainer container, OrgScreeningReportPdfModel model)
    {
        var riskColor = ResolveRiskColor(model.RiskLevel);

        container.BorderLeft(4).BorderColor(riskColor).Background(Surface).Padding(12).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text("AI Assessment Overview")
                .SemiBold()
                .FontSize(13)
                .FontColor(BrandBlue);

            column.Item().Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Risk Level: ").SemiBold();
                    text.Span(string.IsNullOrWhiteSpace(model.RiskLevel) ? "N/A" : model.RiskLevel)
                        .SemiBold()
                        .FontColor(riskColor);
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Confidence Score: ").SemiBold();
                    text.Span(FormatPercentage(model.ConfidenceScore)).SemiBold();
                });
            });

            column.Item().Text(text =>
            {
                text.Span("Assessed At: ").SemiBold();
                text.Span(model.AssessedAt.HasValue
                    ? model.AssessedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    : "N/A");
            });
        });
    }

    private void ComposeAiFindingsTable(IContainer container, OrgScreeningReportPdfModel model)
    {
        container.Border(1).BorderColor(Border).Padding(10).Column(column =>
        {
            column.Spacing(6);
            column.Item().Text("AI Findings Percentage Table")
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
                    columns.ConstantColumn(38);
                    columns.RelativeColumn(3);
                    columns.ConstantColumn(95);
                    columns.ConstantColumn(95);
                });

                table.Header(header =>
                {
                    header.Cell().Element(TableHeaderCell).AlignCenter().Text("#").FontColor(Colors.White).SemiBold();
                    header.Cell().Element(TableHeaderCell).Text("Finding").FontColor(Colors.White).SemiBold();
                    header.Cell().Element(TableHeaderCell).AlignRight().Text("Confidence").FontColor(Colors.White).SemiBold();
                    header.Cell().Element(TableHeaderCell).Text("Status").FontColor(Colors.White).SemiBold();
                });

                foreach (var finding in model.AiFindingDetails.OrderBy(x => x.Rank))
                {
                    table.Cell().Element(TableBodyCell).AlignCenter().Text(finding.Rank.ToString()).FontColor(TextStrong);
                    table.Cell().Element(TableBodyCell).Text(finding.DiseaseName).FontColor(TextStrong);
                    table.Cell().Element(TableBodyCell).AlignRight().Text($"{finding.ConfidencePercentage:0.##}%").FontColor(TextStrong);
                    table.Cell().Element(TableBodyCell).Text(string.IsNullOrWhiteSpace(finding.Status) ? "Detected" : finding.Status).FontColor(TextMuted);
                }
            });
        });
    }

    private void ComposeTextSection(IContainer container, string title, string content)
    {
        container.Border(1).BorderColor(Border).Padding(12).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text(title).SemiBold().FontSize(12).FontColor(BrandBlue);
            column.Item().Text(content).FontSize(10).LineHeight(1.4f);
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
}