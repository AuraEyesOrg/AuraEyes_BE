using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Contracts.Commands.TerminateContract;

public class TerminateContractCommandHandler : ICommandHandler<TerminateContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TerminateContractCommandHandler> _logger;

    public TerminateContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork,
        ILogger<TerminateContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(TerminateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contract is null)
            return Result.NotFound($"Contract {request.Id} not found.");

        try
        {
            contract.Terminate();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Contract {Id} terminated.", request.Id);

        return Result.Success();
    }
}
