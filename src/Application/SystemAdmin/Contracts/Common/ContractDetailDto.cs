namespace Application.SystemAdmin.Contracts.Common;

/// <summary>Detail DTO — includes the stored template file path.</summary>
public class ContractDetailDto : ContractDto
{
    public string? SignedContent { get; set; }
}
