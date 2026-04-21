using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Ophthalmologists.Contracts.UploadSignedContract;

public class UploadSignedContractCommandHandler : ICommandHandler<UploadSignedContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _contractTemplateRepository;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadSignedContractCommandHandler> _logger;

    public UploadSignedContractCommandHandler(
        IContractRepository contractRepository,
        IContractTemplateRepository contractTemplateRepository,
        IIdentityService identityService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<UploadSignedContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _contractTemplateRepository = contractTemplateRepository;
        _identityService = identityService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UploadSignedContractCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (contract is null)
            return Result.NotFound("No contract found for this user.");

        try
        {
            contract.UploadScannedDocument(request.ScannedDocumentUrl);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            var template = await _contractTemplateRepository.GetByIdAsync(contract.TemplateId, cancellationToken);
            var isOrganisationContract = template?.Type == ContractType.MedicalOrganizationContract;

            var notificationTitle = isOrganisationContract
                ? "Tổ chức vừa nộp hợp đồng đã ký"
                : "Bác sĩ vừa nộp hợp đồng đã ký";

            var notificationMessage = isOrganisationContract
                ? "Một tổ chức đã tải lên hợp đồng đã ký. Vui lòng vào Contracts để duyệt."
                : "Một bác sĩ đã tải lên hợp đồng đã ký. Vui lòng vào Contracts để duyệt.";

            var notificationAction = isOrganisationContract
                ? "organisation_contract_submitted"
                : "doctor_contract_submitted";

            var (systemAdmins, _) = await _identityService.GetUsersAsync(
                roleFilter: "SystemAdmin",
                pageNumber: 1,
                pageSize: 1000,
                cancellationToken: cancellationToken);

            var notifyTasks = systemAdmins.Select(admin =>
                _notificationService.SendAsync(
                    admin.Id,
                    notificationTitle,
                    notificationMessage,
                    NotificationType.SystemAlert,
                    payload: new
                    {
                        action = notificationAction,
                        contractId = contract.Id,
                        userId = request.UserId,
                        contractStatus = contract.Status.ToString(),
                        contractType = template?.Type.ToString(),
                        routeHint = "/system-admin/contracts"
                    },
                    cancellationToken: cancellationToken,
                    referenceId: contract.Id));

            await Task.WhenAll(notifyTasks);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to send submitted contract notification for contract {ContractId}.",
                contract.Id);
        }

        _logger.LogInformation("Ophthalmologist uploaded signed contract {ContractId} for user {UserId}.",
            contract.Id, request.UserId);

        return Result.Success();
    }
}
