using Application.Common.Interfaces;
using Application.SystemAdmin.Contracts.Common;

namespace Application.SystemAdmin.Contracts.Commands.SignContract;

public record SignContractCommand : ICommand<ContractDto>
{
    public Guid Id { get; init; }

    /// <summary>Negotiated doctor commission rate in percent (0..100).</summary>
    public decimal CommissionRate { get; init; }

    /// <summary>Negotiated actual monthly salary amount.</summary>
    public decimal ActualMonthlySalary { get; init; }

    /// <summary>
    /// Optional confirmed monthly AI quota limit for organisation contracts.
    /// If omitted, keeps the contract's existing MonthlyQuotaLimit.
    /// </summary>
    public int? ConfirmedMonthlyQuotaLimit { get; init; }

    /// <summary>Optional HTML content of the signed contract.</summary>
    public string? SignedContent { get; init; }

    /// <summary>Optional URL to the scanned/uploaded signed document.</summary>
    public string? ScannedDocumentUrl { get; init; }
}
