using Application.Common.Interfaces;

namespace Application.Consents.Commands.AgreeScreeningConsent;

/// <summary>
/// Command to agree and persist legal consent for a screening session.
/// </summary>
public record AgreeScreeningConsentCommand : ICommand<AgreeScreeningConsentResponse>
{
    public Guid ScreeningId { get; init; }
    public string Content { get; init; } = string.Empty;
}

public record AgreeScreeningConsentResponse
{
    public Guid ConsentId { get; init; }
    public Guid AiScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string Content { get; init; } = string.Empty;
    public bool IsAgreed { get; init; }
    public DateTime? SignedAt { get; init; }
}
