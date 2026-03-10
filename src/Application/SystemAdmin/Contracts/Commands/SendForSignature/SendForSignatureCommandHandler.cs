using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Contracts.Commands.SendForSignature;

public class SendForSignatureCommandHandler : ICommandHandler<SendForSignatureCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SendForSignatureCommandHandler> _logger;

    public SendForSignatureCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork,
        ILogger<SendForSignatureCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SendForSignatureCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contract is null)
            return Result.NotFound($"Contract {request.Id} not found.");

        try
        {
            contract.SendForSignature();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Contract {Id} sent for signature.", request.Id);

        return Result.Success();
    }
}
