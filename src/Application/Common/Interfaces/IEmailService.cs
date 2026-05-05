namespace Application.Common.Interfaces;

/// <summary>
/// Email service interface for sending emails.
/// </summary>
public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default);

    Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends onboarding email to newly created staff members with their credentials.
    /// </summary>
    Task SendStaffOnboardingEmailAsync(string email, string fullName, string temporaryPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends credentials to a walk-in patient created at the clinic.
    /// </summary>
    Task SendPatientWalkInCredentialsAsync(string email, string fullName, string temporaryPassword, CancellationToken cancellationToken = default);

    Task SendClinicAppointmentConfirmationAsync(
        string email,
        ClinicAppointmentConfirmationEmailPayload payload,
        CancellationToken cancellationToken = default);

    Task SendOrganisationScreeningResultShareAsync(
        string email,
        OrganisationScreeningResultShareEmailPayload payload,
        IReadOnlyCollection<EmailAttachment> attachments,
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

public sealed record OrganisationScreeningResultShareEmailPayload(
    Guid ScreeningId,
    string PatientName,
    DateTime CreatedAtUtc,
    string RiskLevel,
    decimal? ConfidenceScore,
    string? Summary,
    bool IncludePdf,
    IReadOnlyCollection<string> RetinalImageUrls
);

public sealed record EmailAttachment(string FileName, byte[] Content, string ContentType = "application/octet-stream");
