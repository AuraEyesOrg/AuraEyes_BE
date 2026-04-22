using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Commands.CreateContract;
using Application.SystemAdmin.Contracts.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Contracts.Commands.SignContract;

public class SignContractCommandHandler : ICommandHandler<SignContractCommand, ContractDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _templateRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly IFullTimeSlotGenerationService _fullTimeSlotGenerationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SignContractCommandHandler> _logger;

    public SignContractCommandHandler(
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IRepository<Organisation> organisationRepository,
        IIdentityService identityService,
        INotificationService notificationService,
        IFullTimeSlotGenerationService fullTimeSlotGenerationService,
        IUnitOfWork unitOfWork,
        ILogger<SignContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _organisationRepository = organisationRepository;
        _identityService = identityService;
        _notificationService = notificationService;
        _fullTimeSlotGenerationService = fullTimeSlotGenerationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractDto>> Handle(
        SignContractCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contract is null)
            return Result<ContractDto>.NotFound($"Contract {request.Id} not found.");

        var template = await _templateRepository.GetByIdAsync(contract.TemplateId, cancellationToken);
        if (template is null)
            return Result<ContractDto>.NotFound($"Contract template {contract.TemplateId} not found.");

        var ophthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(contract.UserId, cancellationToken);
        Organisation? organisation = null;
        var isOrganisationContract = template.Type == ContractType.MedicalOrganizationContract;

        if (isOrganisationContract)
        {
            var organisations = await _organisationRepository.FindAsync(o => o.OwnerId == contract.UserId, cancellationToken);
            organisation = organisations.FirstOrDefault();
            if (organisation is null)
                return Result<ContractDto>.NotFound($"Organisation profile for user {contract.UserId} not found.");
        }

        try
        {
            if (!isOrganisationContract)
            {
                if (ophthalmologist is null)
                    return Result<ContractDto>.NotFound($"Ophthalmologist profile for user {contract.UserId} not found.");

                ophthalmologist.UpdateDealTerms(request.CommissionRate, request.ActualMonthlySalary);
            }
            else
            {
                var resolvedMonthlyQuota = request.ConfirmedMonthlyQuotaLimit ?? contract.MonthlyQuotaLimit;
                if (resolvedMonthlyQuota < 0)
                    return Result<ContractDto>.Failure("Confirmed monthly quota must be non-negative.");

                contract.UpdateMonthlyQuotaLimit(resolvedMonthlyQuota);
                organisation!.ConfigureMonthlyQuota(resolvedMonthlyQuota, DateTime.UtcNow);
            }

            contract.Sign(request.SignedContent, request.ScannedDocumentUrl);
        }
        catch (InvalidOperationException ex)
        {
            return Result<ContractDto>.Failure(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Result<ContractDto>.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!isOrganisationContract
            && ophthalmologist is not null
            && ophthalmologist.EmploymentType == OphthalmologistEmploymentType.FullTime)
        {
            try
            {
                await _fullTimeSlotGenerationService.TriggerForOphthalmologistAsync(
                    ophthalmologist.Id,
                    cancellationToken);
            }
            catch (Exception slotEx)
            {
                _logger.LogWarning(
                    slotEx,
                    "Failed to trigger full-time slot generation after activating contract {ContractId} for ophthalmologist {OphthalmologistId}.",
                    contract.Id,
                    ophthalmologist.Id);
            }
        }

        var user = await _identityService.GetUserByIdAsync(contract.UserId, cancellationToken);

        try
        {
            await _notificationService.SendAsync(
                contract.UserId,
                "Hợp đồng đã được kích hoạt",
                isOrganisationContract
                    ? "Hợp đồng tổ chức của bạn đã được System Admin xác nhận và kích hoạt."
                    : "Hợp đồng bác sĩ của bạn đã được System Admin xác nhận và kích hoạt.",
                NotificationType.SystemAlert,
                payload: new
                {
                    action = "contract_activated",
                    contractId = contract.Id,
                    contractType = template.Type.ToString(),
                    contractStatus = contract.Status.ToString(),
                    routeHint = isOrganisationContract
                        ? "/organisation/contract"
                        : "/ophthalmologist/contract"
                },
                cancellationToken: cancellationToken,
                referenceId: contract.Id);
        }
        catch (Exception notifyEx)
        {
            _logger.LogWarning(
                notifyEx,
                "Failed to send contract activation notification for contract {ContractId} to user {UserId}.",
                contract.Id,
                contract.UserId);
        }

        _logger.LogInformation("Contract {Id} signed and activated.", contract.Id);

        return Result<ContractDto>.Success(CreateContractCommandHandler.ToDto(
            contract,
            template?.Title ?? string.Empty,
            template?.Type.ToString() ?? string.Empty,
            user?.FullName ?? string.Empty,
            user?.Email ?? string.Empty,
            ophthalmologist?.CommissionRate,
            ophthalmologist?.ActualMonthlySalary));
    }
}
