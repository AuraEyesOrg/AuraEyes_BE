namespace Application.Common.Models.Auth;

public record OrganisationRegistrationResponse
{
    public Guid RequestId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}