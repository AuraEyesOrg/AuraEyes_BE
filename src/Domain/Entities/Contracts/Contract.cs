using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Contracts;

/// <summary>
/// Contract entity - signed B2B contracts between the platform and organisations.
/// AiQuotaLimit and PlatformCommissionRate replace the Subscription subsystem.
/// </summary>
public class Contract : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid TemplateId { get; private set; }
    public DateTime? SignedDate { get; private set; }
    public string ContractNumber { get; private set; } = string.Empty;
    public string? ScannedDocumentUrl { get; private set; }
    public string? SignedContent { get; private set; }
    public ContractStatus Status { get; private set; }

    /// <summary>Number of AI screening credits granted under this contract.</summary>
    public int AiQuotaLimit { get; private set; }

    /// <summary>Platform revenue share, e.g. 0.20 = 20%.</summary>
    public decimal PlatformCommissionRate { get; private set; }

    private Contract() { } // EF Core

    public Contract(Guid userId, Guid templateId, string contractNumber,
        int aiQuotaLimit = 0, decimal platformCommissionRate = 0m, string? signedContent = null)
    {
        if (string.IsNullOrWhiteSpace(contractNumber))
            throw new ArgumentException("Contract number cannot be empty", nameof(contractNumber));

        UserId = userId;
        TemplateId = templateId;
        ContractNumber = contractNumber;
        AiQuotaLimit = aiQuotaLimit;
        PlatformCommissionRate = platformCommissionRate;
        SignedContent = signedContent;
        Status = ContractStatus.Draft;
    }

    public void SendForSignature()
    {
        if (Status != ContractStatus.Draft)
            throw new InvalidOperationException("Only draft contracts can be sent for signature");

        Status = ContractStatus.PendingSignature;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Sign(string? signedContent = null, string? scannedDocumentUrl = null)
    {
        if (Status != ContractStatus.PendingSignature)
            throw new InvalidOperationException("Only contracts pending signature can be signed");

        SignedDate = DateTime.UtcNow;
        SignedContent = signedContent ?? SignedContent;
        ScannedDocumentUrl = scannedDocumentUrl;
        Status = ContractStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        if (Status != ContractStatus.Active)
            throw new InvalidOperationException("Only active contracts can expire");

        Status = ContractStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Terminate()
    {
        if (Status != ContractStatus.Active)
            throw new InvalidOperationException("Only active contracts can be terminated");

        Status = ContractStatus.Terminated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ContractStatus.Active || Status == ContractStatus.Expired || Status == ContractStatus.Terminated)
            throw new InvalidOperationException("Cannot cancel active, expired or terminated contracts");

        Status = ContractStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
