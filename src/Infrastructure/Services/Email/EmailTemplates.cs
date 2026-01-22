namespace Infrastructure.Services.Email;

/// <summary>
/// Email template provider for Aura healthcare system.
/// Generates professional, healthcare-appropriate HTML emails in Vietnamese.
/// </summary>
internal static class EmailTemplates
{
    #region Color Palette (from Aura FE index.css)
    
    private const string BrandPrimary = "#13ECEC";      // Vibrant Cyan/Teal
    private const string BrandDark = "#1A202C";         // Dark background
    private const string BrandSoft = "#F0FDFA";         // Soft teal background
    private const string TextMain = "#2D3748";          // Main text color
    private const string TextMuted = "#718096";         // Muted text
    private const string SurfaceWhite = "#FFFFFF";      // White surface
    private const string BorderColor = "#E2E8F0";       // Border color
    private const string BgPrimary = "#F7FAFC";         // Light background
    
    #endregion

    #region Email Subjects

    public const string EmailConfirmationSubject = "[AURA] Xác nhận địa chỉ email – Hệ thống quản lý khám sàng lọc mắt";
    public const string PasswordResetSubject = "[AURA] Yêu cầu đặt lại mật khẩu tài khoản";
    public const string WelcomeSubject = "[AURA] Chào mừng bạn đến với Hệ thống Aura";

    #endregion

    #region Base Template

    private static string WrapInBaseTemplate(string content) => $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <title>Hệ thống Aura</title>
    <!--[if mso]>
    <noscript>
        <xml>
            <o:OfficeDocumentSettings>
                <o:PixelsPerInch>96</o:PixelsPerInch>
            </o:OfficeDocumentSettings>
        </xml>
    </noscript>
    <![endif]-->
    <style>
        /* Reset styles */
        body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
        table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
        img {{ -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }}
        body {{ margin: 0; padding: 0; width: 100% !important; height: 100% !important; }}
        a {{ color: {BrandPrimary}; text-decoration: none; }}
        
        /* Typography */
        body, table, td, p, a, li, blockquote {{
            font-family: 'Inter', 'Segoe UI', system-ui, -apple-system, sans-serif;
        }}
        
        /* Responsive */
        @media only screen and (max-width: 600px) {{
            .container {{ width: 100% !important; padding: 20px !important; }}
            .content {{ padding: 30px 20px !important; }}
            .button {{ width: 100% !important; display: block !important; }}
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; background-color: {BgPrimary};"">
    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BgPrimary};"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""container"" style=""max-width: 600px; background-color: {SurfaceWhite}; border-radius: 16px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);"">
                    
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, {BrandDark} 0%, #0F172A 100%); padding: 30px 40px; border-radius: 16px 16px 0 0;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td>
                                        <h1 style=""margin: 0; color: {SurfaceWhite}; font-size: 24px; font-weight: 700; letter-spacing: -0.5px;"">
                                            <span style=""color: {BrandPrimary};"">AURA</span>
                                        </h1>
                                        <p style=""margin: 8px 0 0 0; color: {TextMuted}; font-size: 14px; font-weight: 500;"">
                                            Hệ thống quản lý khám sàng lọc mắt
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td class=""content"" style=""padding: 40px;"">
                            {content}
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px 40px; border-top: 1px solid {BorderColor}; background-color: {BrandSoft}; border-radius: 0 0 16px 16px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td>
                                        <p style=""margin: 0 0 8px 0; color: {TextMain}; font-size: 14px; font-weight: 600;"">
                                            Hệ thống Aura – Quản lý khám sàng lọc mắt
                                        </p>
                                        <p style=""margin: 0 0 4px 0; color: {TextMuted}; font-size: 13px;"">
                                            Email liên hệ: <a href=""mailto:vietbmt19@gmail.com"" style=""color: {BrandPrimary};"">vietbmt19@gmail.com</a>
                                        </p>
                                        <p style=""margin: 0 0 16px 0; color: {TextMuted}; font-size: 13px;"">
                                            Điện thoại hỗ trợ: (Đang cập nhật)
                                        </p>
                                        <hr style=""border: none; border-top: 1px solid {BorderColor}; margin: 16px 0;"" />
                                        <p style=""margin: 0; color: {TextMuted}; font-size: 12px; font-style: italic;"">
                                            Đây là email được gửi tự động từ hệ thống.<br />
                                            Vui lòng không trả lời trực tiếp email này.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

    #endregion

    #region Email Confirmation Template

    public static string GetEmailConfirmationBody(string confirmationLink)
    {
        var content = $@"
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 20px; font-weight: 600;"">
                Xác nhận địa chỉ email
            </h2>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Kính chào Quý khách,
            </p>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Cảm ơn Quý khách đã đăng ký tài khoản trên <strong>Hệ thống Aura – Quản lý khám sàng lọc mắt</strong>.
            </p>
            
            <p style=""margin: 0 0 24px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Để hoàn tất quá trình đăng ký và kích hoạt tài khoản, vui lòng nhấn vào nút bên dưới để xác nhận địa chỉ email của Quý khách:
            </p>
            
            <!-- CTA Button -->
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 0 0 24px 0;"">
                <tr>
                    <td align=""center"">
                        <a href=""{confirmationLink}"" class=""button"" style=""display: inline-block; background-color: {BrandPrimary}; color: {BrandDark}; font-size: 15px; font-weight: 600; padding: 14px 32px; border-radius: 8px; text-decoration: none; box-shadow: 0 4px 14px rgba(19, 236, 236, 0.25);"">
                            Xác nhận email
                        </a>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 0 0 16px 0; color: {TextMuted}; font-size: 13px; line-height: 1.6;"">
                Nếu nút trên không hoạt động, Quý khách có thể sao chép và dán đường dẫn sau vào trình duyệt:
            </p>
            
            <p style=""margin: 0 0 24px 0; padding: 12px 16px; background-color: {BrandSoft}; border-radius: 8px; word-break: break-all;"">
                <a href=""{confirmationLink}"" style=""color: {BrandPrimary}; font-size: 13px; text-decoration: none;"">
                    {confirmationLink}
                </a>
            </p>
            
            <!-- Security Notice -->
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #FFF5F5; border-radius: 8px; border-left: 4px solid #FC8181;"">
                <tr>
                    <td style=""padding: 16px;"">
                        <p style=""margin: 0 0 8px 0; color: #C53030; font-size: 14px; font-weight: 600;"">
                            Lưu ý bảo mật
                        </p>
                        <p style=""margin: 0; color: {TextMain}; font-size: 13px; line-height: 1.6;"">
                            Nếu Quý khách không thực hiện đăng ký tài khoản này, vui lòng bỏ qua email này. 
                            Đường dẫn xác nhận sẽ tự động hết hạn và tài khoản sẽ không được kích hoạt.
                        </p>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 24px 0 0 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Trân trọng,<br />
                <strong>Hệ thống Aura</strong>
            </p>";

        return WrapInBaseTemplate(content);
    }

    #endregion

    #region Password Reset Template

    public static string GetPasswordResetBody(string resetLink)
    {
        var content = $@"
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 20px; font-weight: 600;"">
                Yêu cầu đặt lại mật khẩu
            </h2>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Kính chào Quý khách,
            </p>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Hệ thống đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của Quý khách trên <strong>Hệ thống Aura – Quản lý khám sàng lọc mắt</strong>.
            </p>
            
            <p style=""margin: 0 0 24px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Để đặt lại mật khẩu, vui lòng nhấn vào nút bên dưới:
            </p>
            
            <!-- CTA Button -->
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 0 0 24px 0;"">
                <tr>
                    <td align=""center"">
                        <a href=""{resetLink}"" class=""button"" style=""display: inline-block; background-color: {BrandPrimary}; color: {BrandDark}; font-size: 15px; font-weight: 600; padding: 14px 32px; border-radius: 8px; text-decoration: none; box-shadow: 0 4px 14px rgba(19, 236, 236, 0.25);"">
                            Đặt lại mật khẩu
                        </a>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 0 0 16px 0; color: {TextMuted}; font-size: 13px; line-height: 1.6;"">
                Nếu nút trên không hoạt động, Quý khách có thể sao chép và dán đường dẫn sau vào trình duyệt:
            </p>
            
            <p style=""margin: 0 0 24px 0; padding: 12px 16px; background-color: {BrandSoft}; border-radius: 8px; word-break: break-all;"">
                <a href=""{resetLink}"" style=""color: {BrandPrimary}; font-size: 13px; text-decoration: none;"">
                    {resetLink}
                </a>
            </p>
            
            <!-- Time Warning -->
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #FFFBEB; border-radius: 8px; border-left: 4px solid #F6AD55; margin-bottom: 16px;"">
                <tr>
                    <td style=""padding: 16px;"">
                        <p style=""margin: 0 0 8px 0; color: #C05621; font-size: 14px; font-weight: 600;"">
                            Thời hạn có hiệu lực
                        </p>
                        <p style=""margin: 0; color: {TextMain}; font-size: 13px; line-height: 1.6;"">
                            Đường dẫn đặt lại mật khẩu này sẽ hết hạn sau <strong>24 giờ</strong> kể từ thời điểm yêu cầu.
                            Sau thời gian này, Quý khách cần thực hiện yêu cầu đặt lại mật khẩu mới.
                        </p>
                    </td>
                </tr>
            </table>
            
            <!-- Security Notice -->
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #FFF5F5; border-radius: 8px; border-left: 4px solid #FC8181;"">
                <tr>
                    <td style=""padding: 16px;"">
                        <p style=""margin: 0 0 8px 0; color: #C53030; font-size: 14px; font-weight: 600;"">
                            Cảnh báo bảo mật
                        </p>
                        <p style=""margin: 0; color: {TextMain}; font-size: 13px; line-height: 1.6;"">
                            Nếu Quý khách <strong>không thực hiện yêu cầu này</strong>, vui lòng bỏ qua email này và không nhấn vào đường dẫn trên.
                            Mật khẩu hiện tại của Quý khách sẽ không bị thay đổi.
                        </p>
                        <p style=""margin: 8px 0 0 0; color: {TextMain}; font-size: 13px; line-height: 1.6;"">
                            Nếu Quý khách nghi ngờ tài khoản đã bị xâm phạm, vui lòng liên hệ ngay với bộ phận hỗ trợ.
                        </p>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 24px 0 0 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Trân trọng,<br />
                <strong>Hệ thống Aura</strong>
            </p>";

        return WrapInBaseTemplate(content);
    }

    #endregion

    #region Welcome Email Template

    public static string GetWelcomeBody(string fullName)
    {
        var displayName = string.IsNullOrWhiteSpace(fullName) ? "Quý khách" : fullName;
        
        var content = $@"
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 20px; font-weight: 600;"">
                Chào mừng đến với Hệ thống Aura
            </h2>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Kính chào <strong>{displayName}</strong>,
            </p>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Chúc mừng Quý khách đã đăng ký thành công tài khoản trên <strong>Hệ thống Aura – Quản lý khám sàng lọc mắt</strong>.
            </p>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Hệ thống Aura cung cấp các tính năng quản lý và theo dõi khám sàng lọc mắt chuyên nghiệp, 
                giúp Quý khách chủ động trong việc chăm sóc sức khỏe thị lực.
            </p>
            
            <!-- Info Box -->
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BrandSoft}; border-radius: 8px; border-left: 4px solid {BrandPrimary}; margin: 24px 0;"">
                <tr>
                    <td style=""padding: 16px;"">
                        <p style=""margin: 0 0 8px 0; color: {TextMain}; font-size: 14px; font-weight: 600;"">
                            Các tính năng chính:
                        </p>
                        <ul style=""margin: 0; padding-left: 20px; color: {TextMain}; font-size: 13px; line-height: 1.8;"">
                            <li>Quản lý lịch khám sàng lọc mắt</li>
                            <li>Theo dõi kết quả khám và lịch sử thăm khám</li>
                            <li>Nhận thông báo nhắc nhở lịch hẹn</li>
                            <li>Tra cứu thông tin sức khỏe thị lực</li>
                        </ul>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Nếu Quý khách có bất kỳ câu hỏi nào, vui lòng liên hệ với bộ phận hỗ trợ qua thông tin bên dưới.
            </p>
            
            <p style=""margin: 24px 0 0 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Trân trọng,<br />
                <strong>Hệ thống Aura</strong>
            </p>";

        return WrapInBaseTemplate(content);
    }

    #endregion
}
