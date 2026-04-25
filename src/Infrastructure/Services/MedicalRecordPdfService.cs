using Application.Common.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services;

public class MedicalRecordPdfService : IMedicalRecordPdfService
{
    static MedicalRecordPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateMedicalRecordPdf(MedicalRecordPdfModel model)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("AURA DIGITAL CLINIC").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text("HỒ SƠ BỆNH ÁN").FontSize(18).SemiBold().AlignCenter();
                        col.Item().Text($"(Mã số: {model.MedicalRecordNumber})").FontSize(10).AlignCenter();
                    });
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    // I. Phần Hành Chính
                    col.Item().Text("I. PHẦN HÀNH CHÍNH").SemiBold().Underline();
                    col.Item().PaddingLeft(10).Column(inner =>
                    {
                        inner.Item().Text($"Họ và tên: {model.PatientName}");
                        inner.Item().Text($"Ngày sinh: {model.DateOfBirth ?? "N/A"}   Giới tính: {model.Gender ?? "N/A"}");
                        inner.Item().Text($"Địa chỉ: {model.Address ?? "N/A"}");
                    });

                    col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // II. Phần Chuyên Môn
                    col.Item().Text("II. PHẦN CHUYÊN MÔN").SemiBold().Underline();
                    col.Item().PaddingLeft(10).Column(inner =>
                    {
                        inner.Item().Text("1. Lý do vào viện:").SemiBold();
                        inner.Item().Text(model.FinalDiagnosis ?? "Khám định kỳ");
                        
                        inner.Item().PaddingTop(5).Text("2. Chẩn đoán xác định:").SemiBold();
                        inner.Item().Text(model.FinalDiagnosis ?? "Chưa có chẩn đoán");

                        inner.Item().PaddingTop(5).Text("3. Hướng điều trị:").SemiBold();
                        inner.Item().Text(model.TreatmentPlan ?? "Theo dõi thêm");
                    });
                    
                    col.Item().PaddingVertical(20).AlignRight().Column(inner => {
                        inner.Item().Text($"Ngày {model.CreatedAt:dd} tháng {model.CreatedAt:MM} năm {model.CreatedAt:yyyy}").Italic();
                        inner.Item().PaddingTop(10).Text("BÁC SĨ ĐIỀU TRỊ").SemiBold().AlignCenter();
                        inner.Item().PaddingTop(40).Text("(Ký và ghi rõ họ tên)").FontSize(9).Italic().AlignCenter();
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Trang ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }
}
