using Application.Common.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;

namespace Infrastructure.Services;

public class MedicalRecordPdfService : IMedicalRecordPdfService
{
    static MedicalRecordPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateMedicalRecordPdf(MedicalRecordPdfModel model)
    {
        var adminData = DeserializeData(model.AdministrativeDataJson);
        var clinicalData = DeserializeData(model.ClinicalDataJson);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

                ComposeHeader(page.Header(), adminData, model);
                ComposeContent(page.Content(), adminData, clinicalData, model);
                ComposeFooter(page.Footer());
            });
        }).GeneratePdf();
    }

    private static Dictionary<string, object> DeserializeData(string? json)
    {
        return string.IsNullOrEmpty(json)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
    }

    private void ComposeHeader(IContainer container, Dictionary<string, object> adminData, MedicalRecordPdfModel model)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("Sở Y tế: .................................").FontSize(7);
                col.Item().Text("Bệnh viện: AURA DIGITAL CLINIC").FontSize(8).SemiBold();
            });

            row.RelativeItem().Column(col =>
            {
                col.Item().Text("BỆNH ÁN MẮT").FontSize(14).SemiBold().AlignCenter();
                col.Item().Text("(Dùng cho điều trị nội trú & ngoại trú)").FontSize(8).Italic().AlignCenter();
                col.Item().Text("MS: 23/BV-01").FontSize(7).AlignCenter();
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text($"Số lưu trữ: {adminData.GetValueOrDefault("soLuuTru") ?? "........"}").FontSize(8);
                col.Item().Text($"Mã YT: {model.MedicalRecordNumber}").FontSize(9).SemiBold();
            });
        });
    }

    private void ComposeContent(IContainer container, Dictionary<string, object> adminData, Dictionary<string, object> clinicalData, MedicalRecordPdfModel model)
    {
        container.PaddingVertical(10).Column(col =>
        {
            ComposeInfoRow(col, adminData);
            ComposeAdministrativeSection(col, adminData, model);
            ComposeManagementSection(col, adminData);
            ComposeDiagnosisSection(col, adminData, model);
            ComposeDischargeSection(col, adminData);
            ComposeMedicalHistorySection(col, adminData, clinicalData);
            ComposeExaminationSection(col, clinicalData);
            ComposeSignatures(col, adminData, model);
        });
    }

    private void ComposeInfoRow(ColumnDescriptor col, Dictionary<string, object> adminData)
    {
        col.Item().Row(r => {
            r.RelativeItem().Text($"Khoa: {adminData.GetValueOrDefault("khoa") ?? "..........."}");
            r.RelativeItem().Text($"Giường: {adminData.GetValueOrDefault("giuong") ?? "..........."}");
        });
        col.Item().PaddingVertical(5).LineHorizontal(1);
    }

    private void ComposeAdministrativeSection(ColumnDescriptor col, Dictionary<string, object> adminData, MedicalRecordPdfModel model)
    {
        col.Item().Text("I. HÀNH CHÍNH").SemiBold().Underline();
        col.Item().PaddingLeft(5).Column(inner =>
        {
            var dob = FormatDate(adminData.GetValueOrDefault("birthDate")?.ToString() ?? model.DateOfBirth);
            var gender = adminData.GetValueOrDefault("gender")?.ToString() ?? model.Gender;

            inner.Item().Row(r => {
                r.RelativeItem(5).Text($"1. Họ và tên: {model.PatientName.ToUpper()}");
                r.RelativeItem(2).Text($"Tuổi: {adminData.GetValueOrDefault("age") ?? "...."}");
            });

            inner.Item().Row(r => {
                r.RelativeItem(1).Text($"2. Ngày sinh: {dob}");
                r.RelativeItem(1).Text($"3. Giới: {(gender?.ToLower() == "nam" ? "[X] Nam" : "[ ] Nam")} / {(gender?.ToLower() == "nữ" ? "[X] Nữ" : "[ ] Nữ")}");
            });

            inner.Item().Row(r => {
                r.RelativeItem().Text($"4. Nghề nghiệp: {adminData.GetValueOrDefault("job") ?? "...."}");
                r.RelativeItem().Text($"5. Dân tộc: {adminData.GetValueOrDefault("ethnicity") ?? "...."}");
                r.RelativeItem().Text($"6. Ngoại kiều: {adminData.GetValueOrDefault("nationality") ?? "...."}");
            });

            inner.Item().Text($"7. Địa chỉ: {adminData.GetValueOrDefault("address") ?? "...."} - {adminData.GetValueOrDefault("ward") ?? "...."} - {adminData.GetValueOrDefault("district") ?? "...."} - {adminData.GetValueOrDefault("province") ?? "...."}");

            inner.Item().Row(r => {
                r.RelativeItem().Text($"8. Nơi làm việc: {adminData.GetValueOrDefault("workplace") ?? "...."}");
                r.RelativeItem().Text($"9. Đối tượng: {adminData.GetValueOrDefault("objectType") ?? "...."}");
            });

            inner.Item().Row(r => {
                r.RelativeItem().Text($"10. BHYT giá trị đến ngày: {FormatDate(adminData.GetValueOrDefault("bhytExpiry")?.ToString())}");
                r.RelativeItem().Text($"Số thẻ BHYT: {adminData.GetValueOrDefault("bhytNumber") ?? "...."}");
            });

            inner.Item().Text($"11. Họ tên, địa chỉ người nhà: {adminData.GetValueOrDefault("relativeName") ?? "...."} - SĐT: {adminData.GetValueOrDefault("relativePhone") ?? "...."}");
        });
    }

    private void ComposeManagementSection(ColumnDescriptor col, Dictionary<string, object> adminData)
    {
        col.Item().PaddingTop(5).Text("II. QUẢN LÝ NGƯỜI BỆNH").SemiBold().Underline();
        col.Item().PaddingLeft(5).Column(inner =>
        {
            inner.Item().Row(r => {
                r.RelativeItem().Text($"12. Vào viện: {adminData.GetValueOrDefault("admissionTime") ?? "...."} giờ ngày {FormatDate(adminData.GetValueOrDefault("admissionDate")?.ToString())}");
                r.RelativeItem().Text($"13. Trực tiếp vào: {adminData.GetValueOrDefault("directEntry") ?? "...."}");
            });
            inner.Item().Row(r => {
                r.RelativeItem().Text($"14. Nơi giới thiệu: {adminData.GetValueOrDefault("referralPlace") ?? "...."}");
                r.RelativeItem().Text($"15. Vào khoa: {adminData.GetValueOrDefault("department") ?? "...."}");
            });
            inner.Item().Row(r => {
                r.RelativeItem().Text($"18. Ra viện: {adminData.GetValueOrDefault("dischargeDate") ?? "...."} - Hình thức: {adminData.GetValueOrDefault("dischargeType") ?? "...."}");
                r.RelativeItem().Text($"19. Tổng số ngày điều trị: {adminData.GetValueOrDefault("totalTreatmentDays") ?? "...."}");
            });
        });
    }

    private void ComposeDiagnosisSection(ColumnDescriptor col, Dictionary<string, object> adminData, MedicalRecordPdfModel model)
    {
        col.Item().PaddingTop(5).Text("III. CHẨN ĐOÁN").SemiBold().Underline();
        col.Item().PaddingLeft(5).Column(inner =>
        {
            inner.Item().Text($"20. Nơi chuyển đến: {adminData.GetValueOrDefault("transferDiagnosis") ?? "........................................"}");
            inner.Item().Text($"21. KKB, Cấp cứu: {adminData.GetValueOrDefault("kkbDiagnosis") ?? "........................................"}");
            inner.Item().Text($"22. Khi vào khoa điều trị: {adminData.GetValueOrDefault("departmentDiagnosis") ?? "........................................"}");
            
            inner.Item().PaddingTop(2).Row(r => {
                r.RelativeItem().Text($"+ Bệnh chính: {model.FinalDiagnosis ?? "...."}");
                r.RelativeItem().Text($"+ Bệnh kèm theo: {adminData.GetValueOrDefault("companionDisease") ?? "...................."}");
            });
            
            inner.Item().Row(r => {
                r.RelativeItem().Text($"+ Chẩn đoán trước PT: {adminData.GetValueOrDefault("preOpDiagnosis") ?? "...................."}");
                r.RelativeItem().Text($"+ Chẩn đoán sau PT: {adminData.GetValueOrDefault("postOpDiagnosis") ?? "...................."}");
            });
        });
    }

    private void ComposeDischargeSection(ColumnDescriptor col, Dictionary<string, object> adminData)
    {
        col.Item().PaddingTop(5).Text("III. TÌNH TRẠNG RA VIỆN").SemiBold().Underline();
        col.Item().PaddingLeft(5).Row(r => {
            r.RelativeItem().Text($"26. Kết quả điều trị: {adminData.GetValueOrDefault("treatmentResult") ?? "...."}");
            r.RelativeItem().Text($"28. Tình hình tử vong: {adminData.GetValueOrDefault("deathTime") ?? "...."} ngày {FormatDate(adminData.GetValueOrDefault("deathDate")?.ToString())}");
        });
    }

    private void ComposeMedicalHistorySection(ColumnDescriptor col, Dictionary<string, object> adminData, Dictionary<string, object> clinicalData)
    {
        col.Item().PaddingTop(10).Text("A. BỆNH ÁN").SemiBold().AlignCenter();
        col.Item().PaddingLeft(5).Column(inner =>
        {
            inner.Item().Text($"I. LÝ DO VÀO VIỆN: {adminData.GetValueOrDefault("admissionReason") ?? "........................................"}");
            inner.Item().Text("II. HỎI BỆNH:");
            inner.Item().PaddingLeft(10).Column(h => {
                h.Item().Text($"1. Quá trình bệnh lý: {adminData.GetValueOrDefault("diseaseProcess") ?? "........................................"}");
                h.Item().Text("2. Tiền sử:");
                h.Item().PaddingLeft(10).Column(p => {
                    p.Item().Text($"- Bản thân: {clinicalData.GetValueOrDefault("medicalHistory") ?? "...."}");
                    p.Item().Text($"- Gia đình: {clinicalData.GetValueOrDefault("familyHistory") ?? "...."}");
                });
            });
        });
    }

    private void ComposeExaminationSection(ColumnDescriptor col, Dictionary<string, object> clinicalData)
    {
        col.Item().PaddingTop(5).Text("III. KHÁM BỆNH").SemiBold();
        col.Item().PaddingLeft(5).Column(exam => {
            ComposeVisionTable(exam, clinicalData);
            ComposeEyeDetailsTable(exam, clinicalData);
        });
    }

    private void ComposeVisionTable(ColumnDescriptor exam, Dictionary<string, object> clinicalData)
    {
        exam.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => {
                columns.RelativeColumn(2);
                columns.RelativeColumn(3);
                columns.RelativeColumn(3);
            });

            table.Header(header => {
                header.Cell().Border(1).Padding(2).Text("Chỉ số").SemiBold();
                header.Cell().Border(1).Padding(2).Text("MẮT PHẢI (MP)").SemiBold();
                header.Cell().Border(1).Padding(2).Text("MẮT TRÁI (MT)").SemiBold();
            });

            AddVisionRow(table, "Thị lực không kính", clinicalData, "rightEyeVisionNoGlass", "leftEyeVisionNoGlass");
            AddVisionRow(table, "Thị lực có kính", clinicalData, "rightEyeVisionWithGlass", "leftEyeVisionWithGlass");
            AddVisionRow(table, "Nhãn áp (mmHg)", clinicalData, "rightEyePressure", "leftEyePressure");
            AddVisionRow(table, "Thị trường", clinicalData, "rightEyeField", "leftEyeField");
        });
    }

    private void AddVisionRow(TableDescriptor table, string label, Dictionary<string, object> data, string rightKey, string leftKey)
    {
        table.Cell().Border(1).Padding(2).Text(label);
        table.Cell().Border(1).Padding(2).Text(data.GetValueOrDefault(rightKey)?.ToString() ?? "....");
        table.Cell().Border(1).Padding(2).Text(data.GetValueOrDefault(leftKey)?.ToString() ?? "....");
    }

    private void ComposeEyeDetailsTable(ColumnDescriptor exam, Dictionary<string, object> clinicalData)
    {
        exam.Item().PaddingTop(10).Table(table => {
            table.ColumnsDefinition(columns => {
                columns.RelativeColumn(2);
                columns.RelativeColumn(3);
                columns.RelativeColumn(3);
            });

            var sections = new Dictionary<string, string> {
                { "miMat", "1. Mi mắt" }, { "ketMac", "2. Kết mạc" }, { "giacMac", "3. Giác mạc" },
                { "cungMac", "4. Củng mạc" }, { "tienPhong", "5. Tiền phòng" }, { "mongMat", "6. Mống mắt" },
                { "theThuyTinh", "7. Thể thủy tinh" }, { "dichKinh", "8. Dịch kính" }, { "vongMac", "9. Võng mạc" }
            };

            foreach (var section in sections) {
                table.Cell().Border(1).Padding(2).Text(section.Value);
                table.Cell().Border(1).Padding(2).Text(GetEyeDetailText(clinicalData, "rightEye", section.Key));
                table.Cell().Border(1).Padding(2).Text(GetEyeDetailText(clinicalData, "leftEye", section.Key));
            }
        });
    }

    private void ComposeSignatures(ColumnDescriptor col, Dictionary<string, object> adminData, MedicalRecordPdfModel model)
    {
        col.Item().PaddingVertical(15).AlignRight().Column(inner => {
            inner.Item().Text($"Ngày {model.CreatedAt:dd} tháng {model.CreatedAt:MM} năm {model.CreatedAt:yyyy}").Italic();
            inner.Item().PaddingTop(5).Text("BÁC SĨ ĐIỀU TRỊ").SemiBold().AlignCenter();
            inner.Item().PaddingTop(30).Text(adminData.GetValueOrDefault("doctorName")?.ToString() ?? "").AlignCenter();
            inner.Item().Text("(Ký và ghi rõ họ tên)").FontSize(8).Italic().AlignCenter();
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x => {
            x.Span("Trang ");
            x.CurrentPageNumber();
            x.Span(" / ");
            x.TotalPages();
        });
    }

    private string FormatDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return "..../..../........";
        return DateTime.TryParse(dateStr, out DateTime dt) ? dt.ToString("dd/MM/yyyy") : dateStr;
    }

    private string GetEyeDetailText(Dictionary<string, object>? clinicalData, string eyeSide, string sectionKey)
    {
        if (clinicalData == null) return "....";
        
        try {
            var clinicalJson = JsonSerializer.Serialize(clinicalData);
            using var doc = JsonDocument.Parse(clinicalJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty(eyeSide, out var eyeObj) || !eyeObj.TryGetProperty(sectionKey, out var section)) 
                return "....";

            if (section.TryGetProperty("normal", out var normalProp) && normalProp.GetBoolean()) 
                return "Bình thường";

            var findings = ExtractFindings(section, sectionKey);
            return findings.Any() ? string.Join(", ", findings) : "Bất thường";
        } catch {
            return "....";
        }
    }

    private List<string> ExtractFindings(JsonElement section, string sectionKey)
    {
        var findings = new List<string>();
        
        if (section.TryGetProperty("checks", out var checks)) {
            foreach (var prop in checks.EnumerateObject()) {
                if (prop.Value.ValueKind == JsonValueKind.True) {
                    findings.Add(GetCheckLabel(sectionKey, prop.Name));
                }
            }
        }

        if (section.TryGetProperty("other", out var otherProp)) {
            var other = otherProp.GetString();
            if (!string.IsNullOrEmpty(other)) findings.Add(other);
        }
        
        return findings;
    }

    private static readonly Dictionary<string, Dictionary<string, string>> CheckLabels = new()
    {
        ["miMat"] = new() { ["phuNe"] = "Phù nề", ["phanUngTheMi"] = "Phản ứng thể mi" },
        ["ketMac"] = new() { ["cuongTuNong"] = "Cương tụ nông", ["cuongTuSau"] = "Cương tụ sâu", ["xuatHuyet"] = "Xuất huyết", ["seoKM"] = "Sẹo KM" },
        ["giacMac"] = new() { ["trong"] = "Trong", ["seo"] = "Sẹo", ["phu"] = "Phù", ["tuaMoi"] = "Tủa mới", ["tuaMoCuu"] = "Tủa mỡ cừu", ["tuaSacTo"] = "Tủa sắc tố", ["tuaCu"] = "Tủa cũ", ["seoGM"] = "Sẹo GM" },
        ["cungMac"] = new() { ["seoCM"] = "Sẹo CM" },
        ["tienPhong"] = new() { ["sauSach"] = "Sâu sạch", ["xepTienPhong"] = "Xẹp tiền phòng", ["xuatHuyet"] = "Xuất huyết", ["mu"] = "Mủ/Xuất tiết", ["tyndall"] = "Tyndall", ["dinh"] = "Dính", ["sacTo"] = "Sắc tố", ["tanMach"] = "Tân mạch" },
        ["mongMat"] = new() { ["thoaiHoa"] = "Thoái hóa", ["tanMachMmongMat"] = "Tân mạch", ["hatKoeppi"] = "Hạt Koeppi", ["hatBusaca"] = "Hạt Busaca", ["tron"] = "Tròn", ["meo"] = "Méo", ["dinh"] = "Dính", ["pxdtCo"] = "PXĐT (+)", ["pxdtKhong"] = "PXĐT (-)", ["gianLiet"] = "Giãn liệt" },
        ["theThuyTinh"] = new() { ["trong"] = "Trong", ["duc"] = "Đục", ["ducVoT3"] = "Đục vỡ T3", ["saLech"] = "Sa lệch", ["raTienPhong"] = "Ra tiền phòng", ["vaoBuongDK"] = "Vào buồng dịch kính", ["dinhSacToMatTruoc"] = "Dính sắc tố", ["viêmMu"] = "Viêm mủ" },
        ["dichKinh"] = new() { ["sach"] = "Sạch", ["tyndall"] = "Tyndall", ["viêmMu"] = "Viêm mủ", ["xuatHuyet"] = "Xuất huyết", ["toChucHoa"] = "Tổ chức hóa", ["bongDKSau"] = "Bong DK sau" },
        ["vongMac"] = new() { ["heMachBT"] = "Hệ mạch BT", ["tacDM"] = "Tắc ĐM", ["tacTM"] = "Tắc TM", ["phu"] = "Phù", ["thieuMau"] = "Thiếu máu", ["tanMachVM"] = "Tân mạch VM", ["diaThiPhu"] = "Đĩa thị phù", ["diaThiTeo"] = "Teo", ["hoangDiemBT"] = "Hoàng điểm BT", ["bongVM"] = "Bong VM", ["rachVM"] = "Rách VM" }
    };

    private string GetCheckLabel(string sectionKey, string checkKey)
    {
        if (CheckLabels.TryGetValue(sectionKey, out var sectionLabels) && sectionLabels.TryGetValue(checkKey, out var label))
            return label;
        return checkKey;
    }
}
