using Application.Common.Interfaces;

namespace Application.OrganisationScreenings.Commands.ShareOrgScreeningResult;

public sealed record ShareOrgScreeningResultCommand : ICommand<ShareOrgScreeningResultResponse>
{
    public required Guid OrgAdminUserId { get; init; }
    public required Guid ScreeningId { get; init; }
    public string? RecipientEmail { get; init; }
    public bool IncludePdf { get; init; } = true;
    public bool IncludeRetinalImages { get; init; }
}

public sealed record ShareOrgScreeningResultResponse
{
    public string RecipientEmail { get; init; } = string.Empty;
    public DateTime SharedAt { get; init; }
}
