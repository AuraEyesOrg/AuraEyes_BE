using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.ContractTemplates.Commands.SetContractTemplateStatus;

public class SetContractTemplateStatusCommandHandler : ICommandHandler<SetContractTemplateStatusCommand>
{
    private readonly IContractTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SetContractTemplateStatusCommandHandler> _logger;

    public SetContractTemplateStatusCommandHandler(
        IContractTemplateRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<SetContractTemplateStatusCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SetContractTemplateStatusCommand request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (template is null)
            return Result.NotFound($"Contract template {request.Id} not found.");

        if (request.IsActive)
            template.Activate();
        else
            template.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Contract template {Id} {Status}.",
            request.Id,
            request.IsActive ? "activated" : "deactivated");

        return Result.Success();
    }
}
