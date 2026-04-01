using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Contracts;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.VerifyOphthalmologist;

/// <summary>
/// Handler for VerifyOphthalmologistCommand.
/// Approves or rejects ophthalmologist credential verification.
/// On approval, creates a contract in PendingSignature status and emails the contract template.
/// </summary>
public class VerifyOphthalmologistCommandHandler : IRequestHandler<VerifyOphthalmologistCommand, Result<string>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _templateRepository;
    private readonly Domain.Common.IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<VerifyOphthalmologistCommandHandler> _logger;

    public VerifyOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        Domain.Common.IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IEmailService emailService,
        INotificationService notificationService,
        ILogger<VerifyOphthalmologistCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _emailService = emailService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(
        VerifyOphthalmologistCommand request,
        CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist == null)
        {
            return Result<string>.Failure("Ophthalmologist not found");
        }

        var reviewFlowType = ophthalmologist.VerificationStatus == VerificationStatus.PendingUpdate
            ? "CredentialUpdateReview"
            : "OnboardingVerification";

        if (request.Approve)
        {
            ophthalmologist.Verify();
            _logger.LogInformation("Ophthalmologist {Id} approved", request.OphthalmologistId);
        }
        else
        {
            ophthalmologist.Reject(request.RejectionReason);
            _logger.LogInformation("Ophthalmologist {Id} rejected. Reason: {Reason}",
                request.OphthalmologistId, request.RejectionReason);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // On approval: create a contract and send email with contract template
        if (request.Approve)
        {
            try
            {
                await CreateContractForOphthalmologistAsync(ophthalmologist, cancellationToken);
            }
            catch (Exception contractEx)
            {
                _logger.LogWarning(contractEx,
                    "Failed to create contract for ophthalmologist {Id}. Approval is still valid.",
                    request.OphthalmologistId);
            }
        }

        // Best-effort: Send notification email to the ophthalmologist
        try
        {
            var userDto = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);
            if (userDto != null)
            {
                await _notificationService.SendAsync(
                    ophthalmologist.UserId,
                    request.Approve ? "Hồ sơ xác minh đã được duyệt" : "Hồ sơ xác minh bị từ chối",
                    request.Approve
                        ? "System Admin đã duyệt hồ sơ xác minh của bạn."
                        : "System Admin đã từ chối hồ sơ xác minh của bạn. Vui lòng xem lý do và cập nhật lại.",
                    NotificationType.SystemAlert,
                    payload: new
                    {
                        action = "verification_review_completed",
                        reviewFlowType,
                        approved = request.Approve,
                        rejectionReason = request.RejectionReason,
                        ophthalmologistId = request.OphthalmologistId
                    },
                    cancellationToken: cancellationToken,
                    referenceId: request.OphthalmologistId);

                if (request.Approve)
                {
                    await _emailService.SendAsync(
                        userDto.Email,
                        "[AURA] Hồ sơ chứng chỉ đã được duyệt - Bước tiếp theo là ký và chốt điều khoản hợp đồng",
                        $"""
                        <h2>Chúc mừng, {userDto.FullName}!</h2>
                        <p>Hồ sơ chứng chỉ hành nghề của bạn đã được xác minh và phê duyệt thành công.</p>
                        <p>Tiếp theo, bạn cần hoàn tất quy trình hợp đồng để chốt điều khoản hợp tác (hoa hồng và lương thực tế):</p>
                        <ol>
                            <li>Đăng nhập vào hệ thống AURA</li>
                            <li>Xem và tải mẫu hợp đồng hợp tác đã được gửi kèm</li>
                            <li>In hợp đồng, ký tên và đóng dấu (nếu có)</li>
                            <li>Chụp ảnh hoặc scan hợp đồng đã ký</li>
                            <li>Upload hợp đồng đã ký lên hệ thống để admin kiểm tra</li>
                        </ol>
                        <p>Sau khi admin xác nhận hợp đồng và hoàn tất chốt Commission Rate + Actual Salary theo thỏa thuận, tài khoản của bạn sẽ được kích hoạt đầy đủ.</p>
                        <p>Bạn vẫn có thể xem lại hợp đồng đã xác nhận trực tiếp trên trang hợp đồng của bác sĩ.</p>
                        <p>— Hệ thống AURA</p>
                        """,
                        isHtml: true,
                        cancellationToken);
                }
                else
                {
                    await _emailService.SendAsync(
                        userDto.Email,
                        "[AURA] Credential Verification Update",
                        $"""
                        <h2>Xin chào, {userDto.FullName}</h2>
                        <p>Hồ sơ chứng chỉ hành nghề của bạn chưa đạt yêu cầu xác minh.</p>
                        {(string.IsNullOrEmpty(request.RejectionReason) ? "" : $"<p><strong>Lý do:</strong> {request.RejectionReason}</p>")}
                        <p>Vui lòng liên hệ đội ngũ hỗ trợ nếu bạn cần thêm thông tin.</p>
                        <p>— Hệ thống AURA</p>
                        """,
                        isHtml: true,
                        cancellationToken);
                }
            }
        }
        catch (Exception emailEx)
        {
            _logger.LogWarning(emailEx,
                "Failed to send verification status email to ophthalmologist {Id}", request.OphthalmologistId);
        }

        return Result<string>.Success(request.Approve
            ? "Ophthalmologist approved successfully"
            : "Ophthalmologist rejected successfully");
    }

    /// <summary>
    /// Creates a contract for the newly approved ophthalmologist.
    /// Finds the active OphthalmologistContract template, creates a Draft contract,
    /// then sends it for signature (→ PendingSignature).
    /// </summary>
    private async Task CreateContractForOphthalmologistAsync(
        Domain.Entities.Users.Ophthalmologist ophthalmologist,
        CancellationToken cancellationToken)
    {
        var userId = ophthalmologist.UserId;

        // Check if a contract already exists for this user
        var existingContract = await _contractRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existingContract != null)
        {
            _logger.LogInformation("Contract already exists for user {UserId}, skipping creation.", userId);
            return;
        }

        // Find the active ophthalmologist contract template
        var (templates, _) = await _templateRepository.GetPagedAsync(
            type: ContractType.OphthalmologistContract,
            isActive: true,
            pageNumber: 1,
            pageSize: 50,
            cancellationToken: cancellationToken);

        var template = SelectTemplateByEmploymentType(templates, ophthalmologist.EmploymentType);
        if (template == null)
        {
            _logger.LogWarning("No active OphthalmologistContract template found. Cannot create contract for user {UserId}.", userId);
            return;
        }

        // Generate contract number: AURA-OPH-{yyyyMMdd}-{random}
        var contractNumber = $"AURA-OPH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        var contract = new Contract(
            userId,
            template.Id,
            contractNumber,
            aiQuotaLimit: 0,
            platformCommissionRate: 0m);

        contract.SendForSignature(); // Draft → PendingSignature

        await _contractRepository.AddAsync(contract, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contract {Number} created and sent for signature for user {UserId}.",
            contractNumber, userId);
    }

    private static ContractTemplate? SelectTemplateByEmploymentType(
        IReadOnlyList<ContractTemplate> templates,
        OphthalmologistEmploymentType employmentType)
    {
        if (templates.Count == 0)
            return null;

        static string Normalize(string value) => value.ToLowerInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);

        var expectedKeyword = employmentType == OphthalmologistEmploymentType.PartTime
            ? "parttime"
            : "fulltime";

        var matched = templates
            .Where(t => t.EmploymentType == employmentType)
            .OrderByDescending(t => t.EffectiveDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.CreatedAt)
            .FirstOrDefault();

        if (matched != null)
            return matched;

        // Fallback: legacy template may not have EmploymentType populated yet.
        return templates
            .Where(t => Normalize(t.Title).Contains(expectedKeyword))
            .OrderByDescending(t => t.EffectiveDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.CreatedAt)
            .FirstOrDefault();
    }
}
