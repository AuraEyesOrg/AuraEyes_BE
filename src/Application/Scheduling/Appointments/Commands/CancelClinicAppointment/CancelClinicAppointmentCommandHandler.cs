using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Application.Common.Constants;

namespace Application.Scheduling.Appointments.Commands.CancelClinicAppointment;

public class CancelClinicAppointmentCommandHandler : ICommandHandler<CancelClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IOrderRepository orderRepository,
        ICurrentUserService currentUser,
        IIdentityService identityService,
        INotificationService notificationService,
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _orderRepository = orderRepository;
        _currentUser = currentUser;
        _identityService = identityService;
        _notificationService = notificationService;
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId is null)
        {
            return Result.Unauthorized("Patient profile is required.");
        }

        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Clinic appointment '{request.AppointmentId}' not found.");
        }

        if (appointment.PatientId != _currentUser.ProfileId.Value)
        {
            return Result.Forbidden("You can only cancel your own clinic appointment.");
        }

        try
        {
            appointment.Cancel(_currentUser.ProfileId.Value, request.Reason);

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

            // Send Notifications
            var patientName = appointment.Patient?.FullName;
            if (string.IsNullOrEmpty(patientName) && appointment.Patient?.UserId != null)
            {
                var user = await _identityService.GetUserByIdAsync(appointment.Patient.UserId.Value, cancellationToken);
                patientName = user?.FullName;
            }
            patientName ??= "Bệnh nhân";

            var notificationTitle = "Lịch hẹn đã bị hủy";
            var notificationMessage = $"Bệnh nhân {patientName} đã hủy lịch hẹn vào lúc {appointment.AppointmentSlot?.StartTime:HH:mm} ngày {appointment.AppointmentSlot?.Date:dd/MM/yyyy}. Lý do: {request.Reason ?? "Không có"}";

            // 1. Notify Admin
            await _notificationService.SendToRoleAsync(
                Roles.SystemAdmin,
                notificationTitle,
                notificationMessage,
                NotificationType.AppointmentCancelled,
                new { AppointmentId = appointment.Id, PatientName = patientName },
                cancellationToken);

            // 2. Notify Receptionists (ClinicStaff role)
            await _notificationService.SendToRoleAsync(
                Roles.ClinicStaff,
                notificationTitle,
                notificationMessage,
                NotificationType.AppointmentCancelled,
                new { AppointmentId = appointment.Id, PatientName = patientName },
                cancellationToken);

            // 3. Notify Assigned Doctor
            var ophthalmologistId = appointment.RequestedDoctorId ?? appointment.AppointmentSlot?.OphthalId;
            if (ophthalmologistId.HasValue)
            {
                var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(ophthalmologistId.Value, cancellationToken);
                if (ophthalmologist != null)
                {
                    await _notificationService.SendAsync(
                        ophthalmologist.UserId,
                        notificationTitle,
                        notificationMessage,
                        NotificationType.AppointmentCancelled,
                        new { AppointmentId = appointment.Id, PatientName = patientName },
                        cancellationToken);
                }
            }

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
