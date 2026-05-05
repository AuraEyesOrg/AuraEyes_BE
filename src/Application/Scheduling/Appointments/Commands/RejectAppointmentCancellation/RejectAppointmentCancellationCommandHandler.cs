using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.RejectAppointmentCancellation;

public class RejectAppointmentCancellationCommandHandler : ICommandHandler<RejectAppointmentCancellationCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public RejectAppointmentCancellationCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(RejectAppointmentCancellationCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Appointment '{request.AppointmentId}' not found.");
        }

        if (appointment.Status != AppointmentStatus.CancellationRequested)
        {
            return Result.Failure($"Appointment is not in CancellationRequested status. Current status: {appointment.Status}");
        }

        // Revert appointment status back to Confirmed
        appointment.RejectCancellation(request.AdminNote);

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to patient
        var targetUserId = appointment.Patient?.UserId;
        if (targetUserId.HasValue)
        {
            await _notificationService.SendAsync(
                targetUserId.Value,
                "Yêu cầu hoàn tiền bị từ chối",
                $"Yêu cầu hoàn tiền cho lịch khám ngày {appointment.AppointmentSlot?.Date:dd/MM/yyyy} đã bị từ chối. Lý do: {request.AdminNote ?? "Vui lòng liên hệ hỗ trợ để biết thêm chi tiết"}.",
                NotificationType.ScheduleChanged,
                new { AppointmentId = appointment.Id, Action = "RefundRejected" },
                cancellationToken,
                appointment.Id);
        }

        return Result.Success();
    }
}
