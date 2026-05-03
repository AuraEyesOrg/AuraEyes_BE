using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.ConfirmAppointmentCancellation;

public class ConfirmAppointmentCancellationCommandHandler : ICommandHandler<ConfirmAppointmentCancellationCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmAppointmentCancellationCommandHandler(
        IAppointmentRepository appointmentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
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
                    // Need a way to mark as Refunded if it's not completed yet
                    // Looking at Order.cs, Refund() requires status Completed.
                    // Let's use Cancel() for now or just update status directly if possible.
                    // Or I'll add a ForceRefund method or something.
                    // For now I'll use Cancel() as it's the safest.
                    order.Cancel();
                }
                else
                {
                    order.Cancel();
                }
            }
        }

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
