namespace Application.Common.Interfaces;

/// <summary>
/// Email service interface for sending emails.
/// </summary>
public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default);

    Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default);

    Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
}
