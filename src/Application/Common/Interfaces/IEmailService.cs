namespace Application.Common.Interfaces;

/// <summary>
/// Email service interface for sending emails.
/// </summary>
public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default);

    Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default);

    Task SendClinicAppointmentConfirmationAsync(
        string email,
        ClinicAppointmentConfirmationEmailPayload payload,
        CancellationToken cancellationToken = default);

    Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);

    Task SendWithAttachmentsAsync(
        string to,
        string subject,
        string body,
        IReadOnlyCollection<EmailAttachment> attachments,
        bool isHtml = true,
        CancellationToken cancellationToken = default);
}

public sealed record ClinicAppointmentConfirmationEmailPayload(
    Guid AppointmentId,
    string PatientName,
    string OrganisationName,
    DateOnly AppointmentDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? VisitReason,
    string CheckInCode,
    string QrPayload
);

public sealed record EmailAttachment(string FileName, byte[] Content, string ContentType = "application/octet-stream");
