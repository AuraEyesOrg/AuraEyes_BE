using Domain.Enums.Network;

namespace Application.Network.Common;

/// <summary>
/// Author information DTO used across network features
/// </summary>
public class AuthorDto
{
    public Guid Id { get; set; }
    public AuthorType AuthorType { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? OrganisationName { get; set; }
}
