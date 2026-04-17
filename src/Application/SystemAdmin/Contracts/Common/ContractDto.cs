namespace Application.SystemAdmin.Contracts.Common;

public class ContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // From template
    public Guid TemplateId { get; set; }
    public string TemplateTitle { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;

    // User info
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public int AiQuotaLimit { get; set; }
    public int MonthlyQuotaLimit { get; set; }
    public decimal PlatformCommissionRate { get; set; }
    public decimal? CommissionRate { get; set; }
    public decimal? ActualMonthlySalary { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? ScannedDocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
