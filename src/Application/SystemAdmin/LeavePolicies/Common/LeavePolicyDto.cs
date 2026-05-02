namespace Application.SystemAdmin.LeavePolicies.Common;

public class LeavePolicyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int AdditionalDays { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
