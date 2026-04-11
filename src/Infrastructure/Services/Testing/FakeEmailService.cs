using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Testing;

/// <summary>
/// Test double for email delivery. No external SMTP call is performed.
/// </summary>
public sealed class FakeEmailService : IEmailService
{
    private readonly ILogger<FakeEmailService> _logger;

    public FakeEmailService(ILogger<FakeEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[FAKE EMAIL] Confirmation email captured for {Email}. Link: {Link}", email, confirmationLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[FAKE EMAIL] Password reset email captured for {Email}. Link: {Link}", email, resetLink);
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[FAKE EMAIL] Welcome email captured for {Email} ({FullName})", email, fullName);
        return Task.CompletedTask;
    }

    public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[FAKE EMAIL] Generic email captured. To={To}, Subject={Subject}", to, subject);
        return Task.CompletedTask;
    }

    public Task SendWithAttachmentsAsync(
        string to,
        string subject,
        string body,
        IReadOnlyCollection<EmailAttachment> attachments,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[FAKE EMAIL] Email with attachments captured. To={To}, Subject={Subject}, Attachments={AttachmentCount}",
            to,
            subject,
            attachments.Count);
        return Task.CompletedTask;
    }
}
