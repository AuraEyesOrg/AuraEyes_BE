namespace Infrastructure.Services.Email;

/// <summary>
/// Email template provider for Aura healthcare system.
/// Generates minimalist, card-based HTML emails in Vietnamese.
/// </summary>
internal static class EmailTemplates
{
    #region Color Palette

    // Brand
    private const string BrandPrimary = "#00E5FF";      // Cyan (Aura Primary)
    private const string BrandDarkText = "#004D56";     // Dark teal for text on primary background
    private const string BrandSoft = "#F0FDFA";         // Soft teal for background blocks
    private const string BrandSoftBorder = "#CCFBF1";   // Border for soft blocks

    // Typography
    private const string TextMain = "#202124";
    private const string TextMuted = "#5F6368";

    // Structure
    private const string SurfaceWhite = "#FFFFFF";
    private const string BorderColor = "#E2E8F0";
    private const string BgPrimary = "#F8F9FA";

    // Status/Alerts
    private const string AlertWarningBg = "#FFFBEB";
    private const string AlertWarningText = "#B7791F";
    private const string BorderWarning = "#F6AD55";

    #endregion

    #region Email Subjects

    public const string EmailConfirmationSubject = "Xác nhận địa chỉ email - Hệ thống Aura";
    public const string PasswordResetSubject = "Yêu cầu đặt lại mật khẩu - Hệ thống Aura";
    public const string WelcomeSubject = "Chào mừng bạn đến với Hệ thống Aura";
    public const string OrganisationOnboardingSubject = "[AURA] Yêu cầu đăng ký tổ chức mới";
    public const string OrganisationAccountProvisionedSubject = "[AURA] Tài khoản tổ chức đã được cấp";

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
    <style>
        body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
        table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
        img {{ -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }}
        body {{ margin: 0; padding: 0; width: 100% !important; height: 100% !important; }}
        a {{ color: {BrandPrimary}; text-decoration: none; }}
        
        body, table, td, p, a, li, blockquote {{
            font-family: 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
        }}
        
        @media only screen and (max-width: 600px) {{
            .container {{ width: 100% !important; padding: 0 !important; border: none !important; border-radius: 0 !important; box-shadow: none !important; }}
            .content {{ padding: 32px 20px !important; }}
            .button {{ width: 100% !important; display: block !important; box-sizing: border-box; text-align: center; }}
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; background-color: {BgPrimary};"">
    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BgPrimary}; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""container"" style=""max-width: 600px; background-color: {SurfaceWhite}; border-top: 6px solid {BrandPrimary}; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); overflow: hidden;"">
                    
                    <tr>
                        <td style=""padding: 32px 40px 16px 40px; text-align: center; border-bottom: 1px solid #F1F3F4;"">
                            <h1 style=""margin: 0; color: {BrandPrimary}; font-size: 26px; font-weight: 700; letter-spacing: 1px;"">
                                ❖ AURA
                            </h1>
                            <p style=""margin: 6px 0 0 0; color: {TextMuted}; font-size: 13px; text-transform: uppercase; letter-spacing: 0.5px;"">
                                Hệ thống quản lý khám sàng lọc mắt
                            </p>
                        </td>
                    </tr>
                    
                    <tr>
                        <td class=""content"" style=""padding: 40px;"">
                            {content}
                        </td>
                    </tr>
                    
                    <tr>
                        <td style=""padding: 24px 40px; background-color: #F8F9FA; border-top: 1px solid {BorderColor}; text-align: center;"">
                            <p style=""margin: 0 0 8px 0; color: {TextMuted}; font-size: 12px; font-weight: 500;"">
                                © 2026 AURA Healthcare System
                            </p>
                            <p style=""margin: 0 0 0 0; color: {TextMuted}; font-size: 12px; line-height: 1.5;"">
                                Email này được tạo tự động, vui lòng không trả lời.<br>
                                Cần hỗ trợ? Liên hệ <a href=""mailto:support@auraeyes.vn"" style=""color: {BrandDarkText}; text-decoration: underline;"">support@auraeyes.vn</a>
                            </p>
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
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 22px; font-weight: 600;"">
                Xác nhận địa chỉ email
            </h2>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Xin chào,
            </p>
            
            <p style=""margin: 0 0 32px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Cảm ơn bạn đã đăng ký tài khoản trên hệ thống <strong>Aura</strong>. Để bảo mật thông tin và kích hoạt tài khoản, vui lòng xác nhận email của bạn.
            </p>
            
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BrandSoft}; border: 1px solid {BrandSoftBorder}; border-radius: 8px; margin: 0 0 32px 0;"">
                <tr>
                    <td align=""center"" style=""padding: 32px 24px;"">
                        <p style=""margin: 0 0 20px 0; color: {BrandDarkText}; font-size: 15px; font-weight: 500;"">
                            Nhấn vào nút bên dưới để hoàn tất đăng ký:
                        </p>
                        <a href=""{confirmationLink}"" class=""button"" style=""display: inline-block; background-color: {BrandPrimary}; color: {BrandDarkText}; font-size: 15px; font-weight: 600; padding: 14px 32px; border-radius: 6px; text-decoration: none; box-shadow: 0 2px 4px rgba(0, 229, 255, 0.2);"">
                            ✓ Xác nhận email
                        </a>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 0 0 8px 0; color: {TextMuted}; font-size: 13px; line-height: 1.6;"">
                Hoặc sao chép đường dẫn này vào trình duyệt của bạn:
            </p>
            
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BgPrimary}; border: 1px dashed {BorderColor}; border-radius: 6px; margin: 0 0 32px 0;"">
                <tr>
                    <td style=""padding: 12px 16px;"">
                        <p style=""margin: 0; color: {TextMuted}; font-size: 12px; line-height: 1.6; word-break: break-all; font-family: 'Courier New', Courier, monospace;"">
                            <a href=""{confirmationLink}"" style=""color: #009CA6; text-decoration: none;"">{confirmationLink}</a>
                        </p>
                    </td>
                </tr>
            </table>
            
            <div style=""border-left: 3px solid #E2E8F0; padding-left: 16px; margin-top: 16px;"">
                <p style=""margin: 0; color: {TextMuted}; font-size: 13px; line-height: 1.5;"">
                    Nếu bạn không tạo tài khoản này, bạn có thể bỏ qua email này. Tài khoản sẽ không được kích hoạt nếu không có sự xác nhận của bạn.
                </p>
            </div>";

        return WrapInBaseTemplate(content);
    }

    #endregion

    #region Password Reset Template

    public static string GetPasswordResetBody(string resetLink)
    {
        var content = $@"
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 22px; font-weight: 600;"">
                Đặt lại mật khẩu
            </h2>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Xin chào,
            </p>
            
            <p style=""margin: 0 0 32px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản Aura của bạn.
            </p>
            
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BrandSoft}; border: 1px solid {BrandSoftBorder}; border-radius: 8px; margin: 0 0 32px 0;"">
                <tr>
                    <td align=""center"" style=""padding: 32px 24px;"">
                        <p style=""margin: 0 0 20px 0; color: {BrandDarkText}; font-size: 15px; font-weight: 500;"">
                            Nhấn vào nút bên dưới để thiết lập mật khẩu mới:
                        </p>
                        <a href=""{resetLink}"" class=""button"" style=""display: inline-block; background-color: {BrandPrimary}; color: {BrandDarkText}; font-size: 15px; font-weight: 600; padding: 14px 32px; border-radius: 6px; text-decoration: none; box-shadow: 0 2px 4px rgba(0, 229, 255, 0.2);"">
                            Thay đổi mật khẩu
                        </a>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 0 0 8px 0; color: {TextMuted}; font-size: 13px; line-height: 1.6;"">
                Hoặc sao chép đường dẫn này vào trình duyệt của bạn:
            </p>
            
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BgPrimary}; border: 1px dashed {BorderColor}; border-radius: 6px; margin: 0 0 32px 0;"">
                <tr>
                    <td style=""padding: 12px 16px;"">
                        <p style=""margin: 0; color: {TextMuted}; font-size: 12px; line-height: 1.6; word-break: break-all; font-family: 'Courier New', Courier, monospace;"">
                            <a href=""{resetLink}"" style=""color: #009CA6; text-decoration: none;"">{resetLink}</a>
                        </p>
                    </td>
                </tr>
            </table>
            
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {AlertWarningBg}; border-left: 3px solid {BorderWarning}; margin-bottom: 24px;"">
                <tr>
                    <td style=""padding: 16px;"">
                        <p style=""margin: 0; color: {AlertWarningText}; font-size: 13px; line-height: 1.5;"">
                            <strong>Lưu ý:</strong> Đường dẫn này chỉ có hiệu lực trong vòng 24 giờ kể từ lúc yêu cầu.
                        </p>
                    </td>
                </tr>
            </table>
            
            <div style=""border-left: 3px solid #E2E8F0; padding-left: 16px;"">
                <p style=""margin: 0; color: {TextMuted}; font-size: 13px; line-height: 1.5;"">
                    Nếu bạn không yêu cầu đổi mật khẩu, vui lòng bỏ qua email này. Mật khẩu của bạn vẫn an toàn và không bị thay đổi.
                </p>
            </div>";

        return WrapInBaseTemplate(content);
    }

    #endregion

    #region Welcome Email Template

    public static string GetWelcomeBody(string fullName)
    {
        var displayName = string.IsNullOrWhiteSpace(fullName) ? "bạn" : fullName;

        var content = $@"
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 22px; font-weight: 600;"">
                Chào mừng đến với Aura
            </h2>
            
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Xin chào <strong>{displayName}</strong>,
            </p>
            
            <p style=""margin: 0 0 24px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Tài khoản của bạn đã được tạo thành công. Chúng tôi rất vui được đồng hành cùng bạn trong việc theo dõi và chăm sóc sức khỏe thị lực.
            </p>
            
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BrandSoft}; border: 1px solid {BrandSoftBorder}; border-radius: 8px; margin: 0 0 32px 0;"">
                <tr>
                    <td style=""padding: 24px;"">
                        <p style=""margin: 0 0 16px 0; color: {BrandDarkText}; font-size: 15px; font-weight: 600;"">
                            Với Aura, bạn có thể:
                        </p>
                        
                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                            <tr>
                                <td width=""24"" valign=""top"" style=""padding: 0 0 12px 0; color: {BrandPrimary};"">❖</td>
                                <td style=""padding: 0 0 12px 0; color: {TextMain}; font-size: 14px; line-height: 1.5;"">Đặt và quản lý lịch hẹn khám sàng lọc dễ dàng</td>
                            </tr>
                            <tr>
                                <td width=""24"" valign=""top"" style=""padding: 0 0 12px 0; color: {BrandPrimary};"">❖</td>
                                <td style=""padding: 0 0 12px 0; color: {TextMain}; font-size: 14px; line-height: 1.5;"">Xem kết quả phân tích và theo dõi tình trạng võng mạc</td>
                            </tr>
                            <tr>
                                <td width=""24"" valign=""top"" style=""padding: 0; color: {BrandPrimary};"">❖</td>
                                <td style=""padding: 0; color: {TextMain}; font-size: 14px; line-height: 1.5;"">Lưu trữ hồ sơ y tế cá nhân an toàn và bảo mật</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <p style=""margin: 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Trân trọng,<br>
                <strong>Đội ngũ Aura</strong>
            </p>";

        return WrapInBaseTemplate(content);
    }

    public static string GetOrganisationOnboardingAdminBody(
        string organisationName,
        string orgType,
        string contactFullName,
        string contactEmail,
        string? contactPhone,
        string? address,
        string? licenseNumber,
        string? notes)
    {
        var content = $@"
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 22px; font-weight: 600;"">
                Yêu cầu đăng ký tổ chức mới
            </h2>
            <p style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Có một tổ chức mới vừa gửi biểu mẫu onboarding và đang chờ System Admin xác nhận.
            </p>
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border: 1px solid {BorderColor}; border-radius: 8px; margin-bottom: 24px;"">
                <tr><td style=""padding: 12px 16px; font-weight: 600; border-bottom: 1px solid {BorderColor}; width: 180px;"">Tên tổ chức</td><td style=""padding: 12px 16px; border-bottom: 1px solid {BorderColor};"">{organisationName}</td></tr>
                <tr><td style=""padding: 12px 16px; font-weight: 600; border-bottom: 1px solid {BorderColor};"">Loại hình</td><td style=""padding: 12px 16px; border-bottom: 1px solid {BorderColor};"">{orgType}</td></tr>
                <tr><td style=""padding: 12px 16px; font-weight: 600; border-bottom: 1px solid {BorderColor};"">Người liên hệ</td><td style=""padding: 12px 16px; border-bottom: 1px solid {BorderColor};"">{contactFullName}</td></tr>
                <tr><td style=""padding: 12px 16px; font-weight: 600; border-bottom: 1px solid {BorderColor};"">Email</td><td style=""padding: 12px 16px; border-bottom: 1px solid {BorderColor};"">{contactEmail}</td></tr>
                <tr><td style=""padding: 12px 16px; font-weight: 600; border-bottom: 1px solid {BorderColor};"">Số điện thoại</td><td style=""padding: 12px 16px; border-bottom: 1px solid {BorderColor};"">{contactPhone ?? "—"}</td></tr>
                <tr><td style=""padding: 12px 16px; font-weight: 600; border-bottom: 1px solid {BorderColor};"">Địa chỉ</td><td style=""padding: 12px 16px; border-bottom: 1px solid {BorderColor};"">{address ?? "—"}</td></tr>
                <tr><td style=""padding: 12px 16px; font-weight: 600; border-bottom: 1px solid {BorderColor};"">Mã giấy phép</td><td style=""padding: 12px 16px; border-bottom: 1px solid {BorderColor};"">{licenseNumber ?? "—"}</td></tr>
                <tr><td style=""padding: 12px 16px; font-weight: 600;"">Ghi chú</td><td style=""padding: 12px 16px;"">{notes ?? "—"}</td></tr>
            </table>
            <div style=""border-left: 3px solid {BorderWarning}; padding-left: 16px; color: {AlertWarningText};"">
                Vui lòng vào giao diện quản trị tổ chức để xác nhận và cấp tài khoản.
            </div>";

        return WrapInBaseTemplate(content);
    }

    public static string GetOrganisationCredentialsBody(
        string contactFullName,
        string organisationName,
        string email,
        string temporaryPassword)
    {
        var content = $@"
            <h2 style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 22px; font-weight: 600;"">
                Tài khoản tổ chức đã được tạo
            </h2>
            <p style=""margin: 0 0 16px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Xin chào <strong>{contactFullName}</strong>,
            </p>
            <p style=""margin: 0 0 20px 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                System Admin đã xác nhận hồ sơ của <strong>{organisationName}</strong>. Dưới đây là tài khoản quản trị tổ chức để bạn đăng nhập vào hệ thống AURA:
            </p>
            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BrandSoft}; border: 1px solid {BrandSoftBorder}; border-radius: 8px; margin-bottom: 24px;"">
                <tr><td style=""padding: 16px;""><p style=""margin: 0 0 8px 0; color: {TextMain}; font-size: 15px;""><strong>Email đăng nhập:</strong> {email}</p><p style=""margin: 0; color: {TextMain}; font-size: 15px;""><strong>Mật khẩu tạm:</strong> {temporaryPassword}</p></td></tr>
            </table>
            <p style=""margin: 0; color: {TextMain}; font-size: 15px; line-height: 1.6;"">
                Sau khi đăng nhập, vui lòng kiểm tra và hoàn tất ký hợp đồng để kích hoạt quyền truy cập đầy đủ.
            </p>";

        return WrapInBaseTemplate(content);
    }

    #endregion
}