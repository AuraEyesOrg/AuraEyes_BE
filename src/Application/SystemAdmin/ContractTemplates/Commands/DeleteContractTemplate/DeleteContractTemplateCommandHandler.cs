using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.ContractTemplates.Commands.DeleteContractTemplate;

public class DeleteContractTemplateCommandHandler : ICommandHandler<DeleteContractTemplateCommand>
{
    private readonly IContractTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteContractTemplateCommandHandler> _logger;

    public DeleteContractTemplateCommandHandler(
        IContractTemplateRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteContractTemplateCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (template is null)
            return Result.NotFound($"Contract template {request.Id} not found.");

        await _repository.DeleteAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contract template {Id} deleted.", request.Id);

        return Result.Success();
    }
}
