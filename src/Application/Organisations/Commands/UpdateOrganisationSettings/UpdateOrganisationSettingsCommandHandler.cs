using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Organisations.Common;
using Domain.Common;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Organisations.Commands.UpdateOrganisationSettings;

public class UpdateOrganisationSettingsCommandHandler
    : ICommandHandler<UpdateOrganisationSettingsCommand, OrganisationSettingsDto>
{
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrganisationSettingsCommandHandler(
        IRepository<Organisation> organisationRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _organisationRepository = organisationRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrganisationSettingsDto>> Handle(
        UpdateOrganisationSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.Query()
            .FirstOrDefaultAsync(o => o.OwnerId == request.OrgAdminUserId, cancellationToken);

        if (organisation is null)
        {
            return Result<OrganisationSettingsDto>.NotFound("Organisation not found.");
        }

        organisation.UpdateDetails(
            request.Name,
            request.Address,
            request.LicenseNumber,
            request.TaxCode,
            request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var owner = await _identityService.GetUserByIdAsync(request.OrgAdminUserId, cancellationToken);
        var ownerDetails = await _identityService.GetUserDetailsAsync(request.OrgAdminUserId, cancellationToken);

        if (owner is null)
        {
            return Result<OrganisationSettingsDto>.Failure("Organisation owner account not found.");
        }

        var contactFullName = string.IsNullOrWhiteSpace(request.ContactFullName)
            ? owner.FullName
            : request.ContactFullName.Trim();

        var contactPhone = string.IsNullOrWhiteSpace(request.ContactPhone)
            ? ownerDetails?.PhoneNumber
            : request.ContactPhone.Trim();

        var contactEmail = string.IsNullOrWhiteSpace(request.ContactEmail)
            ? owner.Email
            : request.ContactEmail.Trim();

        var profileUpdate = await _identityService.UpdateUserProfileAsync(
            request.OrgAdminUserId,
            contactFullName,
            contactPhone,
            ownerDetails?.DateOfBirth,
            ownerDetails?.Gender is null ? null : (int)ownerDetails.Gender.Value,
            request.Address,
            cancellationToken);

        if (!profileUpdate.Succeeded)
        {
            return Result<OrganisationSettingsDto>.Failure(profileUpdate.Errors);
        }

        if (!string.Equals(contactEmail, owner.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailUpdate = await _identityService.UpdateUserEmailAsync(
                request.OrgAdminUserId,
                contactEmail,
                cancellationToken);

            if (!emailUpdate.Succeeded)
            {
                return Result<OrganisationSettingsDto>.Failure(emailUpdate.Errors);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            var avatarUpdate = await _identityService.UpdateAvatarUrlAsync(
                request.OrgAdminUserId,
                request.AvatarUrl,
                cancellationToken);

            if (!avatarUpdate.Succeeded)
            {
                return Result<OrganisationSettingsDto>.Failure(avatarUpdate.Errors);
            }
        }

        var latestOwner = await _identityService.GetUserByIdAsync(request.OrgAdminUserId, cancellationToken);
        var latestOwnerDetails = await _identityService.GetUserDetailsAsync(request.OrgAdminUserId, cancellationToken);

        return Result<OrganisationSettingsDto>.Success(new OrganisationSettingsDto
        {
            OrganisationId = organisation.Id,
            Name = organisation.Name,
            OrgType = organisation.OrgType.ToString(),
            Address = organisation.Address,
            LicenseNumber = organisation.LicenseNumber,
            TaxCode = organisation.TaxCode,
            Description = organisation.Description,
            ContactFullName = latestOwner?.FullName ?? contactFullName,
            ContactEmail = latestOwner?.Email ?? contactEmail,
            ContactPhone = latestOwnerDetails?.PhoneNumber,
            AvatarUrl = latestOwnerDetails?.AvatarUrl
        });
    }
}
