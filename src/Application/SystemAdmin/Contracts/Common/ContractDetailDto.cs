namespace Application.SystemAdmin.Contracts.Common;

/// <summary>Detail DTO — includes the full signed HTML content.</summary>
public class ContractDetailDto : ContractDto
{
    public string? SignedContent { get; set; }
}
