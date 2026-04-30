using Application.Common.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;
using System.Globalization;

namespace Infrastructure.Services;

public class MedicalRecordPdfService : IMedicalRecordPdfService
{
    static MedicalRecordPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateMedicalRecordPdf(MedicalRecordPdfModel model)
    {
        var adminData = string.IsNullOrEmpty(model.AdministrativeDataJson)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(model.AdministrativeDataJson);

        var clinicalData = string.IsNullOrEmpty(model.ClinicalDataJson)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(model.ClinicalDataJson);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

                // Top header
                page.Header().Row(row =>
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
                        col.Item().Text($"Số lưu trữ: {adminData?.GetValueOrDefault("soLuuTru") ?? "........"}").FontSize(8);
                        col.Item().Text($"Mã YT: {model.MedicalRecordNumber}").FontSize(9).SemiBold();
                    });
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    // Info Row: Khoa, Giường
                    col.Item().Row(r => {
                        r.RelativeItem().Text($"Khoa: {adminData?.GetValueOrDefault("khoa") ?? "..........."}");
                        r.RelativeItem().Text($"Giường: {adminData?.GetValueOrDefault("giuong") ?? "..........."}");
                    });

                    col.Item().PaddingVertical(5).LineHorizontal(1);

                    // I. HÀNH CHÍNH
                    col.Item().Text("I. HÀNH CHÍNH").SemiBold().Underline();
                    col.Item().PaddingLeft(5).Column(inner =>
                    {
                        var dobRaw = adminData?.GetValueOrDefault("birthDate")?.ToString() ?? model.DateOfBirth;
                        var dob = FormatDate(dobRaw);
                        var age = adminData?.GetValueOrDefault("age")?.ToString() ?? "....";
                        var gender = adminData?.GetValueOrDefault("gender")?.ToString() ?? model.Gender;

                        inner.Item().Row(r => {
                            r.RelativeItem(5).Text($"1. Họ và tên: {model.PatientName.ToUpper()}");
                            r.RelativeItem(2).Text($"Tuổi: {age}");
                        });

                        inner.Item().Row(r => {
                            r.RelativeItem(1).Text($"2. Ngày sinh: {dob}");
                            r.RelativeItem(1).Text($"3. Giới: {(gender?.ToLower() == "nam" ? "[X] Nam" : "[ ] Nam")} / {(gender?.ToLower() == "nữ" ? "[X] Nữ" : "[ ] Nữ")}");
                        });

                        inner.Item().Row(r => {
                            r.RelativeItem().Text($"4. Nghề nghiệp: {adminData?.GetValueOrDefault("job") ?? "...."}");
                            r.RelativeItem().Text($"5. Dân tộc: {adminData?.GetValueOrDefault("ethnicity") ?? "...."}");
                            r.RelativeItem().Text($"6. Ngoại kiều: {adminData?.GetValueOrDefault("nationality") ?? "...."}");
                        });

                        var address = adminData?.GetValueOrDefault("address")?.ToString();
                        var ward = adminData?.GetValueOrDefault("ward")?.ToString();
                        var district = adminData?.GetValueOrDefault("district")?.ToString();
                        var province = adminData?.GetValueOrDefault("province")?.ToString();
                        
                        inner.Item().Text($"7. Địa chỉ: {address ?? "...."} - {ward ?? "...."} - {district ?? "...."} - {province ?? "...."}");

                        inner.Item().Row(r => {
                            r.RelativeItem().Text($"8. Nơi làm việc: {adminData?.GetValueOrDefault("workplace") ?? "...."}");
                            r.RelativeItem().Text($"9. Đối tượng: {adminData?.GetValueOrDefault("objectType") ?? "...."}");
                        });

                        inner.Item().Row(r => {
                            r.RelativeItem().Text($"10. BHYT giá trị đến ngày: {FormatDate(adminData?.GetValueOrDefault("bhytExpiry")?.ToString())}");
                            r.RelativeItem().Text($"Số thẻ BHYT: {adminData?.GetValueOrDefault("bhytNumber") ?? "...."}");
                        });

                        inner.Item().Text($"11. Họ tên, địa chỉ người nhà: {adminData?.GetValueOrDefault("relativeName") ?? "...."} - SĐT: {adminData?.GetValueOrDefault("relativePhone") ?? "...."}");
                    });

                    // II. QUẢN LÝ NGƯỜI BỆNH
                    col.Item().PaddingTop(5).Text("II. QUẢN LÝ NGƯỜI BỆNH").SemiBold().Underline();
                    col.Item().PaddingLeft(5).Column(inner =>
                    {
                        inner.Item().Row(r => {
                            r.RelativeItem().Text($"12. Vào viện: {adminData?.GetValueOrDefault("admissionTime") ?? "...."} giờ ngày {FormatDate(adminData?.GetValueOrDefault("admissionDate")?.ToString())}");
                            r.RelativeItem().Text($"13. Trực tiếp vào: {adminData?.GetValueOrDefault("directEntry") ?? "...."}");
                        });
                        inner.Item().Row(r => {
                            r.RelativeItem().Text($"14. Nơi giới thiệu: {adminData?.GetValueOrDefault("referralPlace") ?? "...."}");
                            r.RelativeItem().Text($"15. Vào khoa: {adminData?.GetValueOrDefault("department") ?? "...."}");
                        });
                        inner.Item().Row(r => {
                            r.RelativeItem().Text($"18. Ra viện: {adminData?.GetValueOrDefault("dischargeDate") ?? "...."} - Hình thức: {adminData?.GetValueOrDefault("dischargeType") ?? "...."}");
                            r.RelativeItem().Text($"19. Tổng số ngày điều trị: {adminData?.GetValueOrDefault("totalTreatmentDays") ?? "...."}");
                        });
                    });

                    // III. CHẨN ĐOÁN
                    col.Item().PaddingTop(5).Text("III. CHẨN ĐOÁN").SemiBold().Underline();
                    col.Item().PaddingLeft(5).Column(inner =>
                    {
                        inner.Item().Text($"20. Nơi chuyển đến: {adminData?.GetValueOrDefault("transferDiagnosis") ?? "........................................"}");
                        inner.Item().Text($"21. KKB, Cấp cứu: {adminData?.GetValueOrDefault("kkbDiagnosis") ?? "........................................"}");
                        inner.Item().Text($"22. Khi vào khoa điều trị: {adminData?.GetValueOrDefault("departmentDiagnosis") ?? "........................................"}");
                        
                        inner.Item().PaddingTop(2).Row(r => {
                            r.RelativeItem().Text($"+ Bệnh chính: {model.FinalDiagnosis ?? "...."}");
                            r.RelativeItem().Text($"+ Bệnh kèm theo: {adminData?.GetValueOrDefault("companionDisease") ?? "...................."}");
                        });
                        
                        inner.Item().Row(r => {
                            r.RelativeItem().Text($"+ Chẩn đoán trước PT: {adminData?.GetValueOrDefault("preOpDiagnosis") ?? "...................."}");
                            r.RelativeItem().Text($"+ Chẩn đoán sau PT: {adminData?.GetValueOrDefault("postOpDiagnosis") ?? "...................."}");
                        });
                    });

                    // III. TÌNH TRẠNG RA VIỆN
                    col.Item().PaddingTop(5).Text("III. TÌNH TRẠNG RA VIỆN").SemiBold().Underline();
                    col.Item().PaddingLeft(5).Row(r => {
                        r.RelativeItem().Text($"26. Kết quả điều trị: {adminData?.GetValueOrDefault("treatmentResult") ?? "...."}");
                        r.RelativeItem().Text($"28. Tình hình tử vong: {adminData?.GetValueOrDefault("deathTime") ?? "...."} ngày {FormatDate(adminData?.GetValueOrDefault("deathDate")?.ToString())}");
                    });

                    // A. BỆNH ÁN
                    col.Item().PaddingTop(10).Text("A. BỆNH ÁN").SemiBold().AlignCenter();
                    col.Item().PaddingLeft(5).Column(inner =>
                    {
                        inner.Item().Text($"I. LÝ DO VÀO VIỆN: {adminData?.GetValueOrDefault("admissionReason") ?? "........................................"}");
                        inner.Item().Text("II. HỎI BỆNH:");
                        inner.Item().PaddingLeft(10).Column(h => {
                            h.Item().Text($"1. Quá trình bệnh lý: {adminData?.GetValueOrDefault("diseaseProcess") ?? "........................................"}");
                            h.Item().Text("2. Tiền sử:");
                            h.Item().PaddingLeft(10).Column(p => {
                                p.Item().Text($"- Bản thân: {clinicalData?.GetValueOrDefault("medicalHistory") ?? "...."}");
                                p.Item().Text($"- Gia đình: {clinicalData?.GetValueOrDefault("familyHistory") ?? "...."}");
                            });
                        });
                        
                        inner.Item().PaddingTop(5).Text("III. KHÁM BỆNH").SemiBold();
                        inner.Item().PaddingLeft(5).Column(exam => {
                            exam.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(3);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Border(1).Padding(2).Text("Chỉ số").SemiBold();
                                    header.Cell().Border(1).Padding(2).Text("MẮT PHẢI (MP)").SemiBold();
                                    header.Cell().Border(1).Padding(2).Text("MẮT TRÁI (MT)").SemiBold();
                                });

                                table.Cell().Border(1).Padding(2).Text("Thị lực không kính");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("rightEyeVisionNoGlass")?.ToString() ?? "....");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("leftEyeVisionNoGlass")?.ToString() ?? "....");

                                table.Cell().Border(1).Padding(2).Text("Thị lực có kính");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("rightEyeVisionWithGlass")?.ToString() ?? "....");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("leftEyeVisionWithGlass")?.ToString() ?? "....");

                                table.Cell().Border(1).Padding(2).Text("Nhãn áp (mmHg)");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("rightEyePressure")?.ToString() ?? "....");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("leftEyePressure")?.ToString() ?? "....");

                                table.Cell().Border(1).Padding(2).Text("Thị trường");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("rightEyeField")?.ToString() ?? "....");
                                table.Cell().Border(1).Padding(2).Text(clinicalData?.GetValueOrDefault("leftEyeField")?.ToString() ?? "....");
                            });

                            // Eye Examination Details
                            exam.Item().PaddingTop(10).Table(table => {
                                table.ColumnsDefinition(columns => {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(3);
                                });

                                var eyeSections = new Dictionary<string, string> {
                                    { "miMat", "1. Mi mắt" },
                                    { "ketMac", "2. Kết mạc" },
                                    { "giacMac", "3. Giác mạc" },
                                    { "cungMac", "4. Củng mạc" },
                                    { "tienPhong", "5. Tiền phòng" },
                                    { "mongMat", "6. Mống mắt" },
                                    { "theThuyTinh", "7. Thể thủy tinh" },
                                    { "dichKinh", "8. Dịch kính" },
                                    { "vongMac", "9. Võng mạc" }
                                };

                                foreach (var section in eyeSections) {
                                    table.Cell().Border(1).Padding(2).Text(section.Value);
                                    table.Cell().Border(1).Padding(2).Text(GetEyeDetailText(clinicalData, "rightEye", section.Key));
                                    table.Cell().Border(1).Padding(2).Text(GetEyeDetailText(clinicalData, "leftEye", section.Key));
                                }
                            });
                        });
                    });

                    col.Item().PaddingVertical(15).AlignRight().Column(inner => {
                        inner.Item().Text($"Ngày {model.CreatedAt:dd} tháng {model.CreatedAt:MM} năm {model.CreatedAt:yyyy}").Italic();
                        inner.Item().PaddingTop(5).Text("BÁC SĨ ĐIỀU TRỊ").SemiBold().AlignCenter();
                        inner.Item().PaddingTop(30).Text(adminData?.GetValueOrDefault("doctorName")?.ToString() ?? "").AlignCenter();
                        inner.Item().Text("(Ký và ghi rõ họ tên)").FontSize(8).Italic().AlignCenter();
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Trang ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private string FormatDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return "..../..../........";
        if (DateTime.TryParse(dateStr, out DateTime dt))
            return dt.ToString("dd/MM/yyyy");
        return dateStr;
    }

    private string GetEyeDetailText(Dictionary<string, object>? clinicalData, string eyeSide, string sectionKey)
    {
        if (clinicalData == null) return "....";
        
        try {
            var clinicalJson = JsonSerializer.Serialize(clinicalData);
            using var doc = JsonDocument.Parse(clinicalJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty(eyeSide, out var eyeObj)) return "....";
            if (!eyeObj.TryGetProperty(sectionKey, out var section)) return "....";

            if (section.TryGetProperty("normal", out var normalProp) && normalProp.GetBoolean()) 
                return "Bình thường";

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
            
            return findings.Any() ? string.Join(", ", findings) : "Bất thường";
        } catch {
            return "....";
        }
    }

    private string GetCheckLabel(string sectionKey, string checkKey)
    {
        return sectionKey switch
        {
            "miMat" => checkKey switch { "phuNe" => "Phù nề", "phanUngTheMi" => "Phản ứng thể mi", _ => checkKey },
            "ketMac" => checkKey switch { "cuongTuNong" => "Cương tụ nông", "cuongTuSau" => "Cương tụ sâu", "xuatHuyet" => "Xuất huyết", "seoKM" => "Sẹo KM", _ => checkKey },
            "giacMac" => checkKey switch { "trong" => "Trong", "seo" => "Sẹo", "phu" => "Phù", "tuaMoi" => "Tủa mới", "tuaMoCuu" => "Tủa mỡ cừu", "tuaSacTo" => "Tủa sắc tố", "tuaCu" => "Tủa cũ", "seoGM" => "Sẹo GM", _ => checkKey },
            "cungMac" => checkKey switch { "seoCM" => "Sẹo CM", _ => checkKey },
            "tienPhong" => checkKey switch { "sauSach" => "Sâu sạch", "xepTienPhong" => "Xẹp tiền phòng", "xuatHuyet" => "Xuất huyết", "mu" => "Mủ/Xuất tiết", "tyndall" => "Tyndall", "dinh" => "Dính", "sacTo" => "Sắc tố", "tanMach" => "Tân mạch", _ => checkKey },
            "mongMat" => checkKey switch { "thoaiHoa" => "Thoái hóa", "tanMachMmongMat" => "Tân mạch", "hatKoeppi" => "Hạt Koeppi", "hatBusaca" => "Hạt Busaca", "tron" => "Tròn", "meo" => "Méo", "dinh" => "Dính", "pxdtCo" => "PXĐT (+)", "pxdtKhong" => "PXĐT (-)", "gianLiet" => "Giãn liệt", _ => checkKey },
            "theThuyTinh" => checkKey switch { "trong" => "Trong", "duc" => "Đục", "ducVoT3" => "Đục vỡ T3", "saLech" => "Sa lệch", "raTienPhong" => "Ra tiền phòng", "vaoBuongDK" => "Vào buồng dịch kính", "dinhSacToMatTruoc" => "Dính sắc tố", "viêmMu" => "Viêm mủ", _ => checkKey },
            "dichKinh" => checkKey switch { "sach" => "Sạch", "tyndall" => "Tyndall", "viêmMu" => "Viêm mủ", "xuatHuyet" => "Xuất huyết", "toChucHoa" => "Tổ chức hóa", "bongDKSau" => "Bong DK sau", _ => checkKey },
            "vongMac" => checkKey switch { "heMachBT" => "Hệ mạch BT", "tacDM" => "Tắc ĐM", "tacTM" => "Tắc TM", "phu" => "Phù", "thieuMau" => "Thiếu máu", "tanMachVM" => "Tân mạch VM", "diaThiPhu" => "Đĩa thị phù", "diaThiTeo" => "Teo", "hoangDiemBT" => "Hoàng điểm BT", "bongVM" => "Bong VM", "rachVM" => "Rách VM", _ => checkKey },
            _ => checkKey
        };
    }
}
