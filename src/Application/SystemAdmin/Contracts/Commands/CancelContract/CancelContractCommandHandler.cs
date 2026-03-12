using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Contracts.Commands.CancelContract;

public class CancelContractCommandHandler : ICommandHandler<CancelContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelContractCommandHandler> _logger;

    public CancelContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(CancelContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contract is null)
            return Result.NotFound($"Contract {request.Id} not found.");

        try
        {
            contract.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Contract {Id} cancelled.", request.Id);

        return Result.Success();
    }
}
