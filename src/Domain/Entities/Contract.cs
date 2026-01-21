using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Contract entity - signed contracts between parties
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

    private Contract() { } // EF Core

    public Contract(Guid userId, Guid templateId, string contractNumber, string? signedContent = null)
    {
        if (string.IsNullOrWhiteSpace(contractNumber))
            throw new ArgumentException("Contract number cannot be empty", nameof(contractNumber));

        UserId = userId;
        TemplateId = templateId;
        ContractNumber = contractNumber;
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
