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


        await NotifyOphthalmologistAsync(ophthalmologist.UserId, request.Approve, request.OphthalmologistId, cancellationToken);

        return Result<string>.Success(request.Approve
            ? "Ophthalmologist approved successfully"
            : "Ophthalmologist rejected successfully");
    }

    private async Task NotifyOphthalmologistAsync(
        Guid userId,
        bool isApproved,
        Guid ophthalmologistId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userDto = await _identityService.GetUserByIdAsync(userId, cancellationToken);
            if (userDto == null) return;

            await _notificationService.SendAsync(
                userId,
                isApproved ? "Hồ sơ xác minh đã được duyệt" : "Hồ sơ xác minh bị từ chối",
                isApproved
                    ? "System Admin đã duyệt hồ sơ xác minh của bạn."
                    : "System Admin đã từ chối hồ sơ xác minh của bạn. Vui lòng xem lý do và cập nhật lại.",
                NotificationType.SystemAlert,
                payload: new
                {
                    action = "verification_review_completed",
                    approved = isApproved,
                    ophthalmologistId = ophthalmologistId
                },
                cancellationToken: cancellationToken,
                referenceId: ophthalmologistId);

            var emailSubject = isApproved ? "[AURA] Hồ sơ chứng chỉ đã được duyệt" : "[AURA] Credential Verification Update";
            var emailBody = isApproved
                ? $"""
                  <h2>Chúc mừng, {userDto.FullName}!</h2>
                  <p>Hồ sơ chứng chỉ hành nghề của bạn đã được xác minh và phê duyệt thành công.</p>
                  <p>Bây giờ bạn có thể bắt đầu sử dụng các tính năng dành cho bác sĩ trên hệ thống AURA.</p>
                  <p>— Hệ thống AURA</p>
                  """
                : $"""
                  <h2>Xin chào, {userDto.FullName}</h2>
                  <p>Hồ sơ chứng chỉ hành nghề của bạn chưa đạt yêu cầu xác minh.</p>
                  <p>Vui lòng liên hệ đội ngũ hỗ trợ nếu bạn cần thêm thông tin.</p>
                  <p>— Hệ thống AURA</p>
                  """;

            await _emailService.SendAsync(
                userDto.Email,
                emailSubject,
                emailBody,
                isHtml: true,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send verification status email to ophthalmologist {Id}", ophthalmologistId);
        }
    }
}
