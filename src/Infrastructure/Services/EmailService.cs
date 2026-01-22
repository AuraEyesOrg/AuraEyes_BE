using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Email service implementation.
/// TODO: Configure with actual email provider (SendGrid, AWS SES, etc.)
/// Currently logs emails for development.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default)
    {
        // TODO: Replace with actual email sending
        _logger.LogInformation(
            "Email Confirmation - To: {Email}, Link: {Link}",
            email,
            confirmationLink);
        
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default)
    {
        // TODO: Replace with actual email sending
        _logger.LogInformation(
            "Password Reset - To: {Email}, Link: {Link}",
            email,
            resetLink);
        
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default)
    {
        // TODO: Replace with actual email sending
        _logger.LogInformation(
            "Welcome Email - To: {Email}, Name: {Name}",
            email,
            fullName);
        
        return Task.CompletedTask;
    }

    public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        // TODO: Replace with actual email sending
        _logger.LogInformation(
            "Email - To: {To}, Subject: {Subject}, IsHtml: {IsHtml}",
            to,
            subject,
            isHtml);
        
        return Task.CompletedTask;
    }
}
