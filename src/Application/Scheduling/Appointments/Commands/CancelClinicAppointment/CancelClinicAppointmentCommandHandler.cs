using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CancelClinicAppointment;

public class CancelClinicAppointmentCommandHandler : ICommandHandler<CancelClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IOrderRepository orderRepository,
        ICurrentUserService currentUser,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _orderRepository = orderRepository;
        _currentUser = currentUser;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Clinic appointment '{request.AppointmentId}' not found.");
        }

        // Authorization logic:
        // 1. Staff with AppointmentsManage (or SystemAdmin): Can cancel any appointment.
        // 2. Patient: Can only cancel their own.
        
        bool hasManagePermission = false;
        if (_currentUser.UserId.HasValue)
        {
            if (_currentUser.IsInRole(Application.Common.Constants.Roles.SystemAdmin))
            {
                hasManagePermission = true;
            }
            else
            {
                var permissions = await _identityService.GetUserPermissionsAsync(_currentUser.UserId.Value);
                hasManagePermission = permissions.Contains(Application.Common.Constants.Permissions.AppointmentsManage);
            }
        }

        if (!hasManagePermission)
        {
            // If not a staff manager, must be a patient cancelling their own
            if (_currentUser.IsInRole(Application.Common.Constants.Roles.Patient))
            {
                if (_currentUser.ProfileId is null || appointment.PatientId != _currentUser.ProfileId.Value)
                {
                    return Result.Forbidden("You can only cancel your own clinic appointment.");
                }
            }
            else
            {
                // Non-patient staff without Manage permission
                return Result.Forbidden("You do not have permission to manage appointments.");
            }
        }

        try
        {
            var cancelledBy = _currentUser.ProfileId ?? _currentUser.UserId ?? Guid.Empty;
            appointment.Cancel(cancelledBy, request.Reason);

            var slot = appointment.AppointmentSlot;
            if (slot is not null)
            {
                slot.CancelBooking();
                await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
            }

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            // Also cancel linked pending order if exists
            var orders = await _orderRepository.GetByAppointmentIdsAsync(new[] { appointment.Id }, cancellationToken);
            foreach (var order in orders)
            {
                if (order.Status == OrderStatus.Pending)
                {
                    order.Cancel();
                    await _orderRepository.UpdateAsync(order, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
