using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Organisations.Commands.UpdateOrganisation;

/// <summary>
/// Handler for UpdateOrganisationCommand.
/// </summary>
public class UpdateOrganisationCommandHandler : ICommandHandler<UpdateOrganisationCommand>
{
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrganisationCommandHandler(
        IOrganisationRepository organisationRepository,
        IUnitOfWork unitOfWork)
    {
        _organisationRepository = organisationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOrganisationCommand request, CancellationToken cancellationToken)
    {
        // Get the organisation
        var organisation = await _organisationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (organisation is null || organisation.IsDeleted)
        {
            return Result.NotFound($"Organisation with ID '{request.Id}' was not found.");
        }

        // Check if organisation name already exists (excluding current organisation)
        var nameExists = await _organisationRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken);
        if (nameExists)
        {
            return Result.Conflict($"An organisation with name '{request.Name}' already exists.");
        }

        // Check if license number already exists (if provided, excluding current organisation)
        if (!string.IsNullOrWhiteSpace(request.LicenseNumber))
        {
            var licenseExists = await _organisationRepository.ExistsByLicenseNumberAsync(request.LicenseNumber, request.Id, cancellationToken);
            if (licenseExists)
            {
                return Result.Conflict($"An organisation with license number '{request.LicenseNumber}' already exists.");
            }
        }

        // Update the organisation
        organisation.UpdateDetails(request.Name, request.Address, request.LicenseNumber);
        
        // Update org type if changed
        if (organisation.OrgType != request.OrgType)
        {
            organisation.ChangeOrgType(request.OrgType);
        }

        await _organisationRepository.UpdateAsync(organisation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
