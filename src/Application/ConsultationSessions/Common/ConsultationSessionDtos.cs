using Domain.Enums;

namespace Application.ConsultationSessions.Common;

public record ChatMessageDto
{
    public Guid Id { get; init; }
    public Guid SenderUserId { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public DateTime SentAt { get; init; }
}

public record ConsultationSessionDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public Guid? OrganisationId { get; init; }
    public Guid? AiScreeningId { get; init; }
    public string? PatientName { get; init; }
    public string? PatientAvatarUrl { get; init; }
    public string? OphthalmologistName { get; init; }
    public string? OrganisationName { get; init; }
    public string? OphthalmologistAvatarUrl { get; init; }
    public ConsultationSessionType Type { get; init; }
    public string TypeName => Type.ToString();
    public SessionStatus Status { get; init; }
    public string StatusName => Status.ToString();
    public ChatStatus ChatStatus { get; init; }
    public string ChatStatusName => ChatStatus.ToString();
    public decimal Price { get; init; }
    public DateTime? AppointmentTime { get; init; }
    public string? MeetingLink { get; init; }
    public DateTime LastActivityAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public Guid? ClosedBy { get; init; }
    public string? ClosingReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsRetinalImagesShared { get; init; }
    public bool IsAIResultShared { get; init; }
    public ConsultationCaseSnapshotDto? CaseSnapshot { get; init; }
    public IReadOnlyList<ChatMessageDto> Messages { get; init; } = [];
}

public record ConsultationSessionListDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public string? PatientName { get; init; }
    public string? PatientAvatarUrl { get; init; }
    public string? OphthalmologistName { get; init; }
    public string? OrganisationName { get; init; }
    public string? OphthalmologistAvatarUrl { get; init; }
    public ConsultationSessionType Type { get; init; }
    public string TypeName => Type.ToString();
    public SessionStatus Status { get; init; }
    public string StatusName => Status.ToString();
    public ChatStatus ChatStatus { get; init; }
    public string ChatStatusName => ChatStatus.ToString();
    public decimal Price { get; init; }
    public DateTime? AppointmentTime { get; init; }
    public string? MeetingLink { get; init; }
    public DateTime LastActivityAt { get; init; }
    public DateTime CreatedAt { get; init; }

    // Consent flags
    public bool IsRetinalImagesShared { get; init; }
    public bool IsAIResultShared { get; init; }

    // Lightweight AI snapshot for listing/searching (populated in query handler).
    public ConsultationCaseSnapshotDto? CaseSnapshot { get; init; }
}

public record ConsultationCaseSnapshotDto
{
    public Guid ScreeningId { get; init; }
    public string? RiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string? Summary { get; init; }
    public string? Findings { get; init; }
    public string? AnnotatedImageUrl { get; init; }
    public string? RawJsonOutput { get; init; }
    public IReadOnlyList<string> OriginalImageUrls { get; init; } = [];
    public IReadOnlyList<string> Symptoms { get; init; } = [];
}
