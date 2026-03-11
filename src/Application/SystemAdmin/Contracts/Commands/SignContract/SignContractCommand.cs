using Application.Common.Interfaces;
using Application.SystemAdmin.Contracts.Common;

namespace Application.SystemAdmin.Contracts.Commands.SignContract;

public record SignContractCommand : ICommand<ContractDto>
{
    public Guid Id { get; init; }

    /// <summary>Optional HTML content of the signed contract.</summary>
    public string? SignedContent { get; init; }

    /// <summary>Optional URL to the scanned/uploaded signed document.</summary>
    public string? ScannedDocumentUrl { get; init; }
}
