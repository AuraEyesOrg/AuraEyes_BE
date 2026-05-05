using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Application.Common.Constants;

namespace Application.Scheduling.Appointments.Commands.RequestAppointmentCancellation;

public class RequestAppointmentCancellationCommandHandler : ICommandHandler<RequestAppointmentCancellationCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public RequestAppointmentCancellationCommandHandler(
        IAppointmentRepository appointmentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        INotificationService notificationService,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _appointmentRepository = appointmentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _notificationService = notificationService;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result> Handle(RequestAppointmentCancellationCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Appointment '{request.AppointmentId}' not found.");
        }

        // Ownership check
        if (_currentUserService.ProfileId.HasValue && appointment.PatientId != _currentUserService.ProfileId.Value)
        {
             return Result.Forbidden("You do not have permission to cancel this appointment.");
        }

        // 6-hour rule check
        var slot = appointment.AppointmentSlot;
        if (slot == null)
        {
             return Result.Failure("Appointment slot information is missing.");
        }

        var startDateTime = slot.Date.ToDateTime(slot.StartTime);
        if (startDateTime < DateTime.UtcNow.AddHours(6))
        {
            return Result.Failure("Cannot cancel appointment less than 6 hours before it starts.");
        }

        // Update Patient bank info if provided
        if (appointment.Patient != null && !string.IsNullOrWhiteSpace(request.BankNumber))
        {
            appointment.Patient.UpdateBankInfo(request.BankNumber, request.AccountName, request.BankName);
        }

        // Request cancellation
        appointment.RequestCancellation(request.BankNumber, request.AccountName, request.BankName, request.Reason);

        // Update linked orders to reflect refund request if it was paid
        var orders = await _orderRepository.GetByAppointmentIdsAsync(new[] { appointment.Id }, cancellationToken);
        foreach (var order in orders)
        {
            if (order.PaidAmount > 0)
            {
                order.RequestRefund();
            }
            else
            {
                // If not paid at all, we can just cancel it immediately 
                // but usually for appointments we prefer to wait for admin if they want to track it
                // For now, let's just mark it as CancellationRequested too if it's confirmed
                if (order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Pending)
                {
                    order.RequestRefund();
                }
            }
        }

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send Notifications
        var patientName = appointment.Patient?.FullName;
        if (string.IsNullOrEmpty(patientName) && appointment.Patient?.UserId != null)
        {
            var user = await _identityService.GetUserByIdAsync(appointment.Patient.UserId.Value, cancellationToken);
            patientName = user?.FullName;
        }
        patientName ??= "Bệnh nhân";

        var notificationTitle = "Yêu cầu hoàn tiền lịch hẹn";
        var notificationMessage = $"Bệnh nhân {patientName} đã yêu cầu hoàn tiền cho lịch hẹn vào lúc {appointment.AppointmentSlot?.StartTime:HH:mm} ngày {appointment.AppointmentSlot?.Date:dd/MM/yyyy}.";

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
}
