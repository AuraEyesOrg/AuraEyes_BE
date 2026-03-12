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
    private readonly ILogger<VerifyOphthalmologistCommandHandler> _logger;

    public VerifyOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        Domain.Common.IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IEmailService emailService,
        ILogger<VerifyOphthalmologistCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _emailService = emailService;
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
                await CreateContractForOphthalmologistAsync(ophthalmologist.UserId, cancellationToken);
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
                if (request.Approve)
                {
                    await _emailService.SendAsync(
                        userDto.Email,
                        "[AURA] Congratulations! Your Credentials Have Been Approved",
                        $"""
                        <h2>Chúc mừng, {userDto.FullName}!</h2>
                        <p>Hồ sơ chứng chỉ hành nghề của bạn đã được xác minh và phê duyệt thành công.</p>
                        <p>Bước tiếp theo, bạn cần hoàn tất ký hợp đồng hợp tác:</p>
                        <ol>
                            <li>Đăng nhập vào hệ thống AURA</li>
                            <li>Xem và tải mẫu hợp đồng hợp tác đã được gửi kèm</li>
                            <li>In hợp đồng, ký tên và đóng dấu (nếu có)</li>
                            <li>Chụp ảnh hoặc scan hợp đồng đã ký</li>
                            <li>Upload ảnh hợp đồng lên hệ thống</li>
                        </ol>
                        <p>Sau khi admin xác nhận hợp đồng, bạn sẽ được kích hoạt đầy đủ tính năng.</p>
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
    private async Task CreateContractForOphthalmologistAsync(Guid userId, CancellationToken cancellationToken)
    {
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
            pageSize: 1,
            cancellationToken: cancellationToken);

        var template = templates.FirstOrDefault();
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
}
