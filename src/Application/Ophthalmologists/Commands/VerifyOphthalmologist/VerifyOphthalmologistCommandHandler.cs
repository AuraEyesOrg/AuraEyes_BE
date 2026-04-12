using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Ophthalmologists.Commands.VerifyOphthalmologist;

/// <summary>
/// Handler for VerifyOphthalmologistCommand.
/// </summary>
public class VerifyOphthalmologistCommandHandler : ICommandHandler<VerifyOphthalmologistCommand>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyOphthalmologistCommandHandler> _logger;

    public VerifyOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<VerifyOphthalmologistCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(VerifyOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        if (ophthalmologist.IsVerified)
        {
            return Result.Failure("Ophthalmologist is already verified.");
        }

        ophthalmologist.Verify();

        await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _notificationService.SendAsync(
                ophthalmologist.UserId,
                "Tài khoản bác sĩ đã được phê duyệt",
                "Chúc mừng, hồ sơ bác sĩ của bạn đã được System Admin phê duyệt. Bạn có thể tiếp tục sử dụng đầy đủ chức năng chuyên môn trên hệ thống.",
                NotificationType.SystemAlert,
                payload: new
                {
                    action = "ophthalmologist_verification_approved",
                    verificationFlowType = "OnboardingVerification",
                    ophthalmologistId = ophthalmologist.Id
                },
                cancellationToken: cancellationToken,
                referenceId: ophthalmologist.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send approval notification for ophthalmologist {OphthalmologistId}",
                ophthalmologist.Id);
        }

        return Result.Success();
    }
}
