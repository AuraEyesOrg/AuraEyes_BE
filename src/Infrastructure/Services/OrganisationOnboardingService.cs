using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.SystemAdmin.Organisations.Common;
using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Identity;
using Infrastructure.Services.Email;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class OrganisationOnboardingService : IOrganisationOnboardingService
{
    private readonly IRepository<OrganisationOnboardingRequest> _requestRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IRepository<ContractTemplate> _contractTemplateRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AdminNotificationSettings _adminNotificationSettings;
    private readonly ILogger<OrganisationOnboardingService> _logger;

    public OrganisationOnboardingService(
        IRepository<OrganisationOnboardingRequest> requestRepository,
        IRepository<Organisation> organisationRepository,
        IRepository<ContractTemplate> contractTemplateRepository,
        IContractRepository contractRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IOptions<AdminNotificationSettings> adminNotificationSettings,
        ILogger<OrganisationOnboardingService> logger)
    {
        _requestRepository = requestRepository;
        _organisationRepository = organisationRepository;
        _contractTemplateRepository = contractTemplateRepository;
        _contractRepository = contractRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _adminNotificationSettings = adminNotificationSettings.Value;
        _logger = logger;
    }

    public async Task<Result<OrganisationRegistrationResponse>> SubmitRequestAsync(
        RegisterOrganisationRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingPending = await _requestRepository.ExistsAsync(
            r => r.ContactEmail == request.ContactEmail &&
                 r.Status == OrganisationOnboardingStatus.Pending,
            cancellationToken);

        if (existingPending)
        {
            return Result<OrganisationRegistrationResponse>.Conflict(
                "A pending organisation onboarding request already exists for this email.");
        }

        var onboardingRequest = new OrganisationOnboardingRequest(
            request.OrganisationName,
            (OrgType)request.OrgType,
            request.ContactFullName,
            request.ContactEmail,
            request.ContactPhone,
            request.Address,
            request.LicenseNumber,
            request.TaxCode,
            request.Notes);

        await _requestRepository.AddAsync(onboardingRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await NotifyAdminAsync(onboardingRequest, cancellationToken);

        return Result<OrganisationRegistrationResponse>.Success(new OrganisationRegistrationResponse
        {
            RequestId = onboardingRequest.Id,
            Email = onboardingRequest.ContactEmail,
            Message = "Organisation registration submitted successfully. System Admin will review and send account details by email."
        });
    }

    public async Task<Result<IReadOnlyList<OrganisationOnboardingRequestDto>>> GetRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _requestRepository.Query()
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new OrganisationOnboardingRequestDto
            {
                Id = r.Id,
                OrganisationName = r.OrganisationName,
                OrgType = r.OrgType.ToString(),
                ContactFullName = r.ContactFullName,
                ContactEmail = r.ContactEmail,
                ContactPhone = r.ContactPhone,
                Address = r.Address,
                LicenseNumber = r.LicenseNumber,
                TaxCode = r.TaxCode,
                Notes = r.Notes,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt,
                ApprovedAt = r.ApprovedAt
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<OrganisationOnboardingRequestDto>>.Success(items);
    }

    public async Task<Result<ApproveOrganisationOnboardingResult>> ApproveRequestAsync(
        Guid requestId,
        Guid approvedByUserId,
        CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
        {
            return Result<ApproveOrganisationOnboardingResult>.NotFound(
                "Organisation onboarding request not found.");
        }

        if (request.Status != OrganisationOnboardingStatus.Pending)
        {
            return Result<ApproveOrganisationOnboardingResult>.Failure(
                "This onboarding request has already been processed.");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.ContactEmail);
        if (existingUser is not null)
        {
            return Result<ApproveOrganisationOnboardingResult>.Conflict(
                "A user with this organisation contact email already exists.");
        }

        var temporaryPassword = GenerateTemporaryPassword();
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var orgAdmin = new ApplicationUser
            {
                UserName = request.ContactEmail,
                Email = request.ContactEmail,
                FullName = request.ContactFullName,
                Address = request.Address,
                EmailConfirmed = true,
                IsActive = true,
                MustChangePassword = true
            };

            var createResult = await _userManager.CreateAsync(orgAdmin, temporaryPassword);
            if (!createResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ApproveOrganisationOnboardingResult>.Failure(
                    createResult.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(orgAdmin, Roles.OrgAdmin);

            var organisation = new Organisation(
                orgAdmin.Id,
                request.OrganisationName,
                request.OrgType,
                request.Address,
                request.LicenseNumber,
                request.TaxCode);

            await _organisationRepository.AddAsync(organisation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            orgAdmin.OrganizationId = organisation.Id;
            await _userManager.UpdateAsync(orgAdmin);

            var template = await _contractTemplateRepository.Query()
                .Where(t => t.Type == ContractType.MedicalOrganizationContract && t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (template is not null)
            {
                var contractNumber = $"ORG-{DateTime.UtcNow:yyyyMMdd}-{organisation.Id.ToString()[..6].ToUpperInvariant()}";
                var contract = new Contract(orgAdmin.Id, template.Id, contractNumber);
                contract.SendForSignature();
                await _contractRepository.AddAsync(contract, cancellationToken);
            }

            request.Approve(approvedByUserId, organisation.Id, orgAdmin.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _emailService.SendAsync(
                request.ContactEmail,
                EmailTemplates.OrganisationAccountProvisionedSubject,
                EmailTemplates.GetOrganisationCredentialsBody(
                    request.ContactFullName,
                    request.OrganisationName,
                    request.ContactEmail,
                    temporaryPassword),
                isHtml: true,
                cancellationToken);

            return Result<ApproveOrganisationOnboardingResult>.Success(new ApproveOrganisationOnboardingResult
            {
                RequestId = request.Id,
                OrganisationId = organisation.Id,
                OrgAdminUserId = orgAdmin.Id,
                OrgAdminEmail = request.ContactEmail,
                TemporaryPassword = temporaryPassword
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Failed to approve organisation onboarding request {RequestId}", requestId);
            return Result<ApproveOrganisationOnboardingResult>.Failure(
                "Failed to approve organisation onboarding request.");
        }
    }

    private async Task NotifyAdminAsync(
        OrganisationOnboardingRequest request,
        CancellationToken cancellationToken)
    {
        var adminEmails = new List<string>();

        if (!string.IsNullOrWhiteSpace(_adminNotificationSettings.OrganisationOnboardingEmail))
        {
            adminEmails.Add(_adminNotificationSettings.OrganisationOnboardingEmail!);
        }
        else
        {
            var admins = await _userManager.GetUsersInRoleAsync(Roles.SystemAdmin);
            adminEmails.AddRange(
                admins.Where(a => !string.IsNullOrWhiteSpace(a.Email))
                    .Select(a => a.Email!));
        }

        foreach (var email in adminEmails.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var businessCode = ExtractStructuredNoteValue(
                request.Notes,
                "business code",
                "business registration code",
                "mã số doanh nghiệp",
                "ma so doanh nghiep");

            var taxCode = request.TaxCode ?? ExtractStructuredNoteValue(
                request.Notes,
                "tax code",
                "tax id",
                "mã số thuế",
                "ma so thue",
                "mst");

            await _emailService.SendAsync(
                email,
                EmailTemplates.OrganisationOnboardingSubject,
                EmailTemplates.GetOrganisationOnboardingAdminBody(
                    request.OrganisationName,
                    request.OrgType.ToString(),
                    request.ContactFullName,
                    request.ContactEmail,
                    request.ContactPhone,
                    request.Address,
                    request.LicenseNumber,
                    request.Notes,
                    businessCode,
                    taxCode),
                isHtml: true,
                cancellationToken);
        }
    }

    private static string? ExtractStructuredNoteValue(string? notes, params string[] keys)
    {
        if (string.IsNullOrWhiteSpace(notes) || keys.Length == 0)
        {
            return null;
        }

        var normalizedKeys = keys
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k.Trim().ToLowerInvariant())
            .ToList();

        foreach (var line in notes.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.Length == 0)
            {
                continue;
            }

            var lowerLine = trimmedLine.ToLowerInvariant();

            foreach (var key in normalizedKeys)
            {
                if (!lowerLine.StartsWith(key))
                {
                    continue;
                }

                var separatorIndex = trimmedLine.IndexOf(':');
                if (separatorIndex < 0)
                {
                    separatorIndex = trimmedLine.IndexOf('=');
                }

                if (separatorIndex < 0 || separatorIndex + 1 >= trimmedLine.Length)
                {
                    continue;
                }

                var value = trimmedLine[(separatorIndex + 1)..].Trim();
                if (value.Length > 0)
                {
                    return value;
                }
            }
        }

        return null;
    }

    private static string GenerateTemporaryPassword()
        => $"Aura@{Guid.NewGuid().ToString("N")[..10]}1!";
}
