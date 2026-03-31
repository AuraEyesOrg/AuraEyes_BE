using Application.Common.Interfaces;

namespace Application.Auth.Queries.GetProfileClaimsByUserId;

public record GetProfileClaimsByUserIdQuery(Guid UserId, IReadOnlyList<string> Roles)
    : IQuery<ProfileClaimsDto>;

public record ProfileClaimsDto
{
    public Guid? ProfileId { get; init; }
    public bool? IsVerified { get; init; }
    public string? VerificationStatus { get; init; }
}
