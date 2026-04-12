using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.OrganisationPatients.Commands.CreateWalkInPatient;

public class CreateWalkInPatientCommandHandler : ICommandHandler<CreateWalkInPatientCommand, Guid>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<OrganisationPatientLink> _organisationPatientLinkRepository;
    private readonly IRepository<Organisation> _orgRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateWalkInPatientCommandHandler> _logger;

    public CreateWalkInPatientCommandHandler(
        IRepository<Patient> patientRepository,
        IRepository<OrganisationPatientLink> organisationPatientLinkRepository,
        IRepository<Organisation> orgRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<CreateWalkInPatientCommandHandler> logger)
    {
        _patientRepository = patientRepository;
        _organisationPatientLinkRepository = organisationPatientLinkRepository;
        _orgRepository = orgRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateWalkInPatientCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;
        if (adminId is null)
        {
            return Result<Guid>.Unauthorized("User not authenticated");
        }

        var orgs = await _orgRepository.FindAsync(o => o.OwnerId == adminId.Value, cancellationToken);
        var org = orgs.FirstOrDefault();

        if (org is null)
        {
            return Result<Guid>.NotFound("Organisation not found");
        }

        var phoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : request.PhoneNumber.Trim();

        var citizenId = string.IsNullOrWhiteSpace(request.CitizenId)
            ? null
            : request.CitizenId.Trim();

        var address = string.IsNullOrWhiteSpace(request.Address)
            ? null
            : request.Address.Trim();

        // ── Duplicate checks scoped to organisation via OrganisationPatientLink → Patient ──

        if (phoneNumber is not null)
        {
            var phoneInUse = await _organisationPatientLinkRepository.Query()
                .Where(link => link.OrganisationId == org.Id && !link.IsDeleted)
                .Join(
                    _patientRepository.Query().Where(p => p.PhoneNumber == phoneNumber && !p.IsDeleted),
                    link => link.PatientId,
                    patient => patient.Id,
                    (link, patient) => patient)
                .AnyAsync(cancellationToken);

            if (phoneInUse)
            {
                return Result<Guid>.Conflict("Phone number already exists in this organisation");
            }
        }

        if (citizenId is not null)
        {
            var citizenIdInUse = await _organisationPatientLinkRepository.Query()
                .Where(link => link.OrganisationId == org.Id && !link.IsDeleted)
                .Join(
                    _patientRepository.Query().Where(p => p.CitizenId == citizenId && !p.IsDeleted),
                    link => link.PatientId,
                    patient => patient.Id,
                    (link, patient) => patient)
                .AnyAsync(cancellationToken);

            if (citizenIdInUse)
            {
                return Result<Guid>.Conflict("Citizen ID already exists in this organisation");
            }
        }

        // ── Parse gender ──

        int? genderId = string.IsNullOrWhiteSpace(request.Gender) ? null
            : request.Gender.StartsWith("M", StringComparison.OrdinalIgnoreCase) ? 1
            : request.Gender.StartsWith("F", StringComparison.OrdinalIgnoreCase) ? 2 : 3;

        // ── Create walk-in patient (no Identity user) ──

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var patient = Patient.CreateWalkIn(
                fullName: request.FullName,
                phoneNumber: phoneNumber,
                citizenId: citizenId,
                dateOfBirth: request.DateOfBirth,
                genderId: genderId,
                address: address);

            await _patientRepository.AddAsync(patient, cancellationToken);
            await _organisationPatientLinkRepository.AddAsync(
                new OrganisationPatientLink(org.Id, patient.Id, "walk-in"),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Walk-in patient {PatientId} created by OrgAdmin {AdminId}",
                patient.Id, adminId);

            return Result<Guid>.Success(patient.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error creating walk-in patient profile");
            return Result<Guid>.Failure($"Failed to create patient profile: {ex.Message}");
        }
    }
}
