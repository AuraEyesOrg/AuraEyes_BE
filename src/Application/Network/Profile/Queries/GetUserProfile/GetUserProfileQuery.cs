using Application.Common.Interfaces;

namespace Application.Network.Profile.Queries.GetUserProfile;

/// <summary>
/// Query to retrieve public profile information for a network user
/// </summary>
public record GetUserProfileQuery : IQuery<UserProfileDto>
{
    public Guid UserId { get; init; }
}
