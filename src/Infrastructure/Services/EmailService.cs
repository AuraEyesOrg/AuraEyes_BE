using Application.Common.Interfaces;
using Infrastructure.Services.Email;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using QRCoder;
using MimeKit.Utils;

namespace Infrastructure.Services;

/// <summary>
/// SMTP email service implementation using MailKit + MimeKit.
/// Sends professional, healthcare-appropriate HTML emails for the Aura system.
/// </summary>
public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default)
    {
        var subject = EmailTemplates.EmailConfirmationSubject;
        var body = EmailTemplates.GetEmailConfirmationBody(confirmationLink);

        await SendAsync(email, subject, body, isHtml: true, cancellationToken);

        _logger.LogInformation(
            "Email confirmation sent to {Email}",
            MaskEmail(email));
    }

    /// <inheritdoc />
    public async Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default)
    {
        var subject = EmailTemplates.PasswordResetSubject;
        var body = EmailTemplates.GetPasswordResetBody(resetLink);

        await SendAsync(email, subject, body, isHtml: true, cancellationToken);

        _logger.LogInformation(
            "Password reset email sent to {Email}",
            MaskEmail(email));
    }

    /// <inheritdoc />
    public async Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default)
    {
        var subject = EmailTemplates.WelcomeSubject;
        var body = EmailTemplates.GetWelcomeBody(fullName);

        await SendAsync(email, subject, body, isHtml: true, cancellationToken);

        _logger.LogInformation(
            "Welcome email sent to {Email}",
            MaskEmail(email));
    }

    /// <inheritdoc />
    public async Task SendClinicAppointmentConfirmationAsync(
        string email,
        ClinicAppointmentConfirmationEmailPayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentNullException.ThrowIfNull(payload);

        var qrCodeBytes = CreateQrCodePngBytes(payload.QrPayload);
        var qrContentId = MimeUtils.GenerateMessageId();

        var subject = EmailTemplates.ClinicAppointmentConfirmationSubject;
        var body = EmailTemplates.GetClinicAppointmentConfirmationBody(
            payload.PatientName,
            payload.OrganisationName,
            payload.AppointmentDate,
            payload.StartTime,
            payload.EndTime,
            payload.VisitReason,
            payload.AppointmentId,
            payload.CheckInCode,
            $"cid:{qrContentId}");

        var message = CreateMessageWithInlineImage(
            email,
            subject,
            body,
            qrCodeBytes,
            qrContentId);

        try
        {
            await SendMessageAsync(message, cancellationToken);

            _logger.LogInformation(
                "Clinic appointment confirmation email sent to {Email} for appointment {AppointmentId}",
                MaskEmail(email),
                payload.AppointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send clinic appointment confirmation email - To: {To}, AppointmentId: {AppointmentId}",
                MaskEmail(email),
                payload.AppointmentId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendOrganisationScreeningResultShareAsync(
        string email,
        OrganisationScreeningResultShareEmailPayload payload,
        IReadOnlyCollection<EmailAttachment> attachments,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(attachments);

        var subject = EmailTemplates.GetOrganisationScreeningResultShareSubject(payload.ScreeningId);
        var body = EmailTemplates.GetOrganisationScreeningResultShareBody(
            payload.PatientName,
            payload.ScreeningId,
            payload.CreatedAtUtc,
            payload.RiskLevel,
            payload.Summary,
            payload.IncludePdf,
            payload.RetinalImageUrls);

        var message = CreateMessage(email, subject, body, isHtml: true, attachments);

        try
        {
            await SendMessageAsync(message, cancellationToken);

            _logger.LogInformation(
                "Organisation screening result share email sent to {Email} for screening {ScreeningId}",
                MaskEmail(email),
                payload.ScreeningId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send organisation screening result share email - To: {To}, ScreeningId: {ScreeningId}",
                MaskEmail(email),
                payload.ScreeningId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to, nameof(to));
        ArgumentException.ThrowIfNullOrWhiteSpace(subject, nameof(subject));
        ArgumentException.ThrowIfNullOrWhiteSpace(body, nameof(body));

        var message = CreateMessage(to, subject, body, isHtml, []);

        try
        {
            await SendMessageAsync(message, cancellationToken);

            _logger.LogDebug(
                "Email sent successfully - To: {To}, Subject: {Subject}",
                MaskEmail(to),
                subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email - To: {To}, Subject: {Subject}, Error: {Error}",
                MaskEmail(to),
                subject,
                ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendWithAttachmentsAsync(
        string to,
        string subject,
        string body,
        IReadOnlyCollection<EmailAttachment> attachments,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to, nameof(to));
        ArgumentException.ThrowIfNullOrWhiteSpace(subject, nameof(subject));
        ArgumentException.ThrowIfNullOrWhiteSpace(body, nameof(body));

        var message = CreateMessage(to, subject, body, isHtml, attachments);

        try
        {
            await SendMessageAsync(message, cancellationToken);

            _logger.LogDebug(
                "Email with attachments sent successfully - To: {To}, Subject: {Subject}, AttachmentCount: {AttachmentCount}",
                MaskEmail(to),
                subject,
                attachments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email with attachments - To: {To}, Subject: {Subject}, Error: {Error}",
                MaskEmail(to),
                subject,
                ex.Message);
            throw;
        }
    }

    #region Private Methods

    private MimeMessage CreateMessage(
        string to,
        string subject,
        string body,
        bool isHtml,
        IReadOnlyCollection<EmailAttachment> attachments)
    {
        var message = new MimeMessage();

        // From address with display name
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));

        // To address
        message.To.Add(MailboxAddress.Parse(to));

        // Subject
        message.Subject = subject;

        // Body (HTML or plain text)
        var bodyBuilder = new BodyBuilder();
        if (isHtml)
        {
            bodyBuilder.HtmlBody = body;
        }
        else
        {
            bodyBuilder.TextBody = body;
        }

        foreach (var attachment in attachments)
        {
            if (attachment.Content.Length == 0 || string.IsNullOrWhiteSpace(attachment.FileName))
            {
                continue;
            }

            bodyBuilder.Attachments.Add(
                attachment.FileName,
                attachment.Content,
                ContentType.Parse(attachment.ContentType));
        }

        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }

    private MimeMessage CreateMessageWithInlineImage(
        string to,
        string subject,
        string htmlBody,
        byte[] imageContent,
        string imageContentId)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };

        var inlineImage = bodyBuilder.LinkedResources.Add(
            $"clinic-appointment-qr-{Guid.NewGuid():N}.png",
            imageContent,
            ContentType.Parse("image/png"));
        inlineImage.ContentId = imageContentId;
        inlineImage.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    private async Task SendMessageAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            // Determine secure socket options
            var secureSocketOptions = DetermineSecureSocketOptions();

            _logger.LogDebug(
                "Connecting to SMTP server {Host}:{Port} with {SecurityOption}",
                _settings.Host,
                _settings.Port,
                secureSocketOptions);

            // Connect to SMTP server
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                secureSocketOptions,
                cancellationToken);

            // Authenticate if credentials provided
            if (!string.IsNullOrWhiteSpace(_settings.Username) &&
                !string.IsNullOrWhiteSpace(_settings.Password))
            {
                await client.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password,
                    cancellationToken);
            }

            // Send the message
            await client.SendAsync(message, cancellationToken);
        }
        finally
        {
            // Disconnect gracefully
            if (client.IsConnected)
            {
                await client.DisconnectAsync(quit: true, cancellationToken);
            }
        }
    }

    private SecureSocketOptions DetermineSecureSocketOptions()
    {
        // StartTLS is preferred for port 587
        if (_settings.UseStartTls)
        {
            return SecureSocketOptions.StartTls;
        }

        // SSL/TLS for port 465
        if (_settings.UseSsl)
        {
            return SecureSocketOptions.SslOnConnect;
        }

        // Auto-detect (not recommended for production)
        return SecureSocketOptions.Auto;
    }

    private static byte[] CreateQrCodePngBytes(string payload)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payload, nameof(payload));

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(8);
    }

    /// <summary>
    /// Masks email for logging (privacy protection).
    /// Example: jo***@example.com
    /// </summary>
    private static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "[empty]";

        var parts = email.Split('@');
        if (parts.Length != 2)
            return "[invalid]";

        var localPart = parts[0];
        var domain = parts[1];

        var maskedLocal = localPart.Length <= 2
            ? new string('*', localPart.Length)
            : $"{localPart[..2]}{new string('*', Math.Min(localPart.Length - 2, 3))}";

        return $"{maskedLocal}@{domain}";
    }

    #endregion
}

