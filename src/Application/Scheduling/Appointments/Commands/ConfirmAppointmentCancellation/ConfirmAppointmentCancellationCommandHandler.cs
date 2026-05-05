using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Application.Common.Constants;

namespace Application.Scheduling.Appointments.Commands.ConfirmAppointmentCancellation;

public class ConfirmAppointmentCancellationCommandHandler : ICommandHandler<ConfirmAppointmentCancellationCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;

    public ConfirmAppointmentCancellationCommandHandler(
        IAppointmentRepository appointmentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        INotificationService notificationService)
    {
        _appointmentRepository = appointmentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(ConfirmAppointmentCancellationCommand request, CancellationToken cancellationToken)
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

        // Update appointment status to Cancelled
        var adminId = _currentUserService.UserId ?? Guid.Empty;

        appointment.Cancel(adminId, "Refund processed by admin.");

        // Also release the slot count
        if (appointment.AppointmentSlot != null)
        {
            appointment.AppointmentSlot.CancelBooking();
        }

        // Update linked orders
        var orders = await _orderRepository.GetByAppointmentIdsAsync(new[] { request.AppointmentId }, cancellationToken);
        foreach (var order in orders)
        {
            if (order.Status != OrderStatus.Cancelled && order.Status != OrderStatus.Refunded)
            {
                // If it was paid, mark as Refunded, else Cancelled
                if (order.PaidAmount > 0)
                {
                    var refundAmount = order.PaidAmount;
                    var refundDesc = string.IsNullOrWhiteSpace(request.AdminNote) 
                        ? "Refund for appointment cancellation" 
                        : $"Refund: {request.AdminNote}";
                    
                    var refundPayment = new Domain.Entities.Financial.Payment(order.Id, refundAmount, PaymentMethod.BankTransfer, refundDesc);
                    // To get to Refunded state via domain logic, we often need it completed first
                    refundPayment.Complete(request.RefundTransactionId);
                    refundPayment.Refund();
                    order.AddPayment(refundPayment);

                    order.Refund();
                }
                else
                {
                    order.Cancel();
                }
            }
        }

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify Patient
        if (appointment.Patient?.UserId != null)
        {
            var title = "Hoàn tiền lịch hẹn thành công";
            var message = $"Yêu cầu hoàn tiền cho lịch hẹn vào lúc {appointment.AppointmentSlot?.StartTime:HH:mm} ngày {appointment.AppointmentSlot?.Date:dd/MM/yyyy} đã được xử lý.";

            await _notificationService.SendAsync(
                appointment.Patient.UserId.Value,
                title,
                message,
                NotificationType.RefundProcessed,
                new { AppointmentId = appointment.Id },
                cancellationToken);
        }

        return Result.Success();
    }
}
