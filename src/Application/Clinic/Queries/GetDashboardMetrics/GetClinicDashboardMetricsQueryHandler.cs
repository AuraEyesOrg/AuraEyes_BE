using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Clinic.Queries.GetDashboardMetrics;

public record GetClinicDashboardMetricsQuery : IQuery<ClinicDashboardMetricsDto>;

public class GetClinicDashboardMetricsQueryHandler : IQueryHandler<GetClinicDashboardMetricsQuery, ClinicDashboardMetricsDto>
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IIdentityService _identityService;

    public GetClinicDashboardMetricsQueryHandler(
        IRepository<Appointment> appointmentRepository,
        IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
    }

    public async Task<Result<ClinicDashboardMetricsDto>> Handle(GetClinicDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        // Use UTC+7 for Vietnamese timezone as the default for this project
        var vnTime = DateTime.UtcNow.AddHours(7);
        var today = DateOnly.FromDateTime(vnTime);

        // Get today's appointments
        var todayAppointments = await _appointmentRepository.Query().AsNoTracking()
            .Include(a => a.AppointmentSlot)
            .Include(a => a.Patient)
            .Where(a => a.AppointmentSlot.Date == today && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        // Resolve patient names in batch
        var patientUserIds = todayAppointments
            .Where(a => a.Patient != null && a.Patient.UserId.HasValue)
            .Select(a => a.Patient!.UserId!.Value)
            .Distinct()
            .ToList();

        var users = await _identityService.GetUsersByIdsAsync(patientUserIds, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => u.FullName ?? "Unknown");

        var metrics = new ClinicDashboardMetricsDto
        {
            TodayAppointments = todayAppointments.Count,
            CheckedInPatients = todayAppointments.Count(a => a.Status == AppointmentStatus.CheckedIn),
            CompletedToday = todayAppointments.Count(a => a.Status == AppointmentStatus.Completed),
            PendingTasks = todayAppointments.Count(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed),
            RecentActivity = todayAppointments
                .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
                .Take(10)
                .Select(a => {
                    string patientName = "Guest Patient";
                    if (a.Patient != null)
                    {
                        if (a.Patient.UserId.HasValue && userMap.TryGetValue(a.Patient.UserId.Value, out var name))
                        {
                            patientName = name;
                        }
                        else if (!string.IsNullOrWhiteSpace(a.Patient.FullName))
                        {
                            patientName = a.Patient.FullName;
                        }
                    }

                    return new ClinicActivityDto
                    {
                        PatientName = patientName,
                        Action = GetActionLabel(a.Status, patientName),
                        Time = a.UpdatedAt ?? a.CreatedAt,
                        Status = GetStatusLabel(a.Status)
                    };
                }).ToList()
        };

        return Result<ClinicDashboardMetricsDto>.Success(metrics);
    }

    private static string GetActionLabel(AppointmentStatus status, string name) => status switch
    {
        AppointmentStatus.Pending => "New appointment booked",
        AppointmentStatus.Confirmed => "Appointment confirmed",
        AppointmentStatus.CheckedIn => "Patient checked in at counter",
        AppointmentStatus.InProgress => "Consultation in progress",
        AppointmentStatus.Completed => "Treatment finalized",
        AppointmentStatus.Cancelled => "Appointment cancelled",
        _ => "Status updated"
    };

    private static string GetStatusLabel(AppointmentStatus status) => status switch
    {
        AppointmentStatus.Completed => "completed",
        AppointmentStatus.CheckedIn => "urgent",
        AppointmentStatus.InProgress => "urgent",
        _ => "pending"
    };
}

