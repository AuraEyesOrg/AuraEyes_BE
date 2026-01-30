using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Organisations.Commands.CreateOrganisation;

/// <summary>
/// Handler for CreateOrganisationCommand.
/// </summary>
public class CreateOrganisationCommandHandler : ICommandHandler<CreateOrganisationCommand, Guid>
{
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrganisationCommandHandler(
        IOrganisationRepository organisationRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _organisationRepository = organisationRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
    {
        // Verify that the owner user exists
        var owner = await _identityService.GetUserByIdAsync(request.OwnerId, cancellationToken);
        if (owner is null)
        {
            return Result<Guid>.NotFound($"User with ID '{request.OwnerId}' was not found.");
        }

        // Check if organisation name already exists
        var nameExists = await _organisationRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken);
        if (nameExists)
        {
            return Result<Guid>.Conflict($"An organisation with name '{request.Name}' already exists.");
        }

        // Check if license number already exists (if provided)
        if (!string.IsNullOrWhiteSpace(request.LicenseNumber))
        {
            var licenseExists = await _organisationRepository.ExistsByLicenseNumberAsync(request.LicenseNumber, cancellationToken: cancellationToken);
            if (licenseExists)
            {
                return Result<Guid>.Conflict($"An organisation with license number '{request.LicenseNumber}' already exists.");
            }
        }

        // Create the organisation entity
        var organisation = new Organisation(
            request.OwnerId,
            request.Name,
            request.OrgType,
            request.Address,
            request.LicenseNumber);

        await _organisationRepository.AddAsync(organisation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(organisation.Id);
    }
}
