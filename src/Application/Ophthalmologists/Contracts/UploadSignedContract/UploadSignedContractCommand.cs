using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Contracts.UploadSignedContract;

/// <summary>
/// Command to upload a scanned/signed contract document.
/// The ophthalmologist prints, signs, photographs, and uploads the contract.
/// </summary>
public record UploadSignedContractCommand : ICommand
{
    public Guid UserId { get; init; }
    public string ScannedDocumentUrl { get; init; } = string.Empty;
}
