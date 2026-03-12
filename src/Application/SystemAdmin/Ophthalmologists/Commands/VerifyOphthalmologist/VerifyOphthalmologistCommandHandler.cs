using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.VerifyOphthalmologist;

/// <summary>
/// Handler for VerifyOphthalmologistCommand.
/// Approves or rejects ophthalmologist credential verification.
/// </summary>
public class VerifyOphthalmologistCommandHandler : IRequestHandler<VerifyOphthalmologistCommand, Result<string>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly Domain.Common.IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly ILogger<VerifyOphthalmologistCommandHandler> _logger;

    public VerifyOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        Domain.Common.IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IEmailService emailService,
        ILogger<VerifyOphthalmologistCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
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

        // Entity was fetched via FindAsync → already tracked by EF Change Tracking.
        // Calling UpdateAsync would force _dbSet.Update() on a tracked entity and
        // generate an INSERT instead of an UPDATE (PK_Ophthalmologists duplicate key).
        // Simply SaveChanges — EF will detect the dirty properties and emit UPDATE.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
                        <p>Bạn có thể bắt đầu:</p>
                        <ul>
                            <li>Nhận ca sàng lọc AI từ bệnh nhân</li>
                            <li>Tư vấn trực tuyến có phí qua hệ thống AURA</li>
                        </ul>
                        <p>Hãy đăng nhập lại để truy cập Dashboard của bạn.</p>
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
}
