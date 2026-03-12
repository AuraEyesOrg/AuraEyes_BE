using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Ophthalmologists.Contracts.UploadSignedContract;

public class UploadSignedContractCommandHandler : ICommandHandler<UploadSignedContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadSignedContractCommandHandler> _logger;

    public UploadSignedContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork,
        ILogger<UploadSignedContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UploadSignedContractCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (contract is null)
            return Result.NotFound("No contract found for this user.");

        try
        {
            contract.UploadScannedDocument(request.ScannedDocumentUrl);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ophthalmologist uploaded signed contract {ContractId} for user {UserId}.",
            contract.Id, request.UserId);

        return Result.Success();
    }
}
