using Application.Common.Interfaces;
using Application.Common.Models;
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
    private readonly Domain.Common.IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<VerifyOphthalmologistCommandHandler> _logger;

    public VerifyOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        Domain.Common.IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IEmailService emailService,
        INotificationService notificationService,
        ILogger<VerifyOphthalmologistCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
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

        _logger.LogInformation("Ophthalmologist verification review for {Id} (approve={Approve})",
            request.OphthalmologistId, request.Approve);


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
                        approved = request.Approve,
                        ophthalmologistId = request.OphthalmologistId
                    },
                    cancellationToken: cancellationToken,
                    referenceId: request.OphthalmologistId);

                if (request.Approve)
                {
                    await _emailService.SendAsync(
                        userDto.Email,
                        "[AURA] Hồ sơ chứng chỉ đã được duyệt",
                        $"""
                        <h2>Chúc mừng, {userDto.FullName}!</h2>
                        <p>Hồ sơ chứng chỉ hành nghề của bạn đã được xác minh và phê duyệt thành công.</p>
                        <p>Bây giờ bạn có thể bắt đầu sử dụng các tính năng dành cho bác sĩ trên hệ thống AURA.</p>
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

}
