using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Clinic.Queries.GetDashboardMetrics;

public record GetClinicDashboardMetricsQuery : IQuery<ClinicDashboardMetricsDto>;

public class GetClinicDashboardMetricsQueryHandler : IQueryHandler<GetClinicDashboardMetricsQuery, ClinicDashboardMetricsDto>
{
    private readonly IRepository<Appointment> _appointmentRepository;

    public GetClinicDashboardMetricsQueryHandler(IRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<ClinicDashboardMetricsDto>> Handle(GetClinicDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Get today's appointments
        var todayAppointments = await _appointmentRepository.Query()
            .Include(a => a.AppointmentSlot)
            .Where(a => a.AppointmentSlot.Date == today && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        var metrics = new ClinicDashboardMetricsDto
        {
            TodayAppointments = todayAppointments.Count,
            CheckedInPatients = todayAppointments.Count(a => a.Status == AppointmentStatus.CheckedIn),
            CompletedToday = todayAppointments.Count(a => a.Status == AppointmentStatus.Completed),
            PendingTasks = todayAppointments.Count(a => a.Status == AppointmentStatus.Pending),
            RecentActivity = todayAppointments
                .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
                .Take(10)
                .Select(a => new ClinicActivityDto
                {
                    PatientName = "Patient", // In a real app, join with Patient -> User
                    Action = GetActionLabel(a.Status),
                    Time = a.UpdatedAt ?? a.CreatedAt,
                    Status = GetStatusLabel(a.Status)
                }).ToList()
        };

        return Result<ClinicDashboardMetricsDto>.Success(metrics);
    }

    private static string GetActionLabel(AppointmentStatus status) => status switch
    {
        AppointmentStatus.CheckedIn => "Patient checked in",
        AppointmentStatus.InProgress => "Consultation started",
        AppointmentStatus.Completed => "Visit completed",
        AppointmentStatus.Cancelled => "Appointment cancelled",
        _ => "Status updated"
    };

    private static string GetStatusLabel(AppointmentStatus status) => status switch
    {
        AppointmentStatus.Completed => "completed",
        AppointmentStatus.Pending => "pending",
        _ => "pending"
    };
}
