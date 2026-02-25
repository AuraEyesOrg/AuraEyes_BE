using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IConsultationSessionService
{
    Task<Result<Guid>> CreateVerificationSessionAsync(
        Guid patientId,
        Guid aiScreeningId,
        decimal price,
        Guid? ophthalmologistId = null,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateVideoCallSessionAsync(
        Guid patientId,
        decimal price,
        DateTime appointmentTime,
        Guid? ophthalmologistId = null,
        string? meetingLink = null,
        CancellationToken cancellationToken = default);

    Task<Result> SubmitVerificationReportAsync(
        Guid sessionId,
        Guid doctorId,
        string diagnosesCode,
        string diagnosesText,
        string? treatmentPlan = null,
        CancellationToken cancellationToken = default);

    Task<Result> SendMessageAsync(
        Guid sessionId,
        Guid senderUserId,
        string message,
        CancellationToken cancellationToken = default);

    Task<Result> EndSessionAsync(
        Guid sessionId,
        Guid doctorId,
        CancellationToken cancellationToken = default);
}
