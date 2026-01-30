using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Organisations.Commands.DeleteOrganisation;

/// <summary>
/// Handler for DeleteOrganisationCommand.
/// Performs soft-delete on the organisation.
/// </summary>
public class DeleteOrganisationCommandHandler : ICommandHandler<DeleteOrganisationCommand>
{
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrganisationCommandHandler(
        IOrganisationRepository organisationRepository,
        IUnitOfWork unitOfWork)
    {
        _organisationRepository = organisationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteOrganisationCommand request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (organisation is null || organisation.IsDeleted)
        {
            return Result.NotFound($"Organisation with ID '{request.Id}' was not found.");
        }

        // Soft delete
        organisation.Delete();
        
        await _organisationRepository.UpdateAsync(organisation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
