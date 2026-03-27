using Application.AiQuota.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using Application.Organisations.Queries.GetDashboardMetrics;
using Application.Patients.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;
using Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;
using Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;
using Application.SystemAdmin.Dashboard.Queries.GetSystemHealth;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class DashboardMetricsService : IDashboardMetricsService
{
    private readonly ApplicationDbContext _context;
    private readonly IAiQuotaService _aiQuotaService;

    public DashboardMetricsService(ApplicationDbContext context, IAiQuotaService aiQuotaService)
    {
        _context = context;
        _aiQuotaService = aiQuotaService;
    }

    public async Task<DashboardMetricsDto> GetSystemAdminMetricsAsync(CancellationToken cancellationToken = default)
    {
        var completedDeposits = await _context.DepositRequests
            .Where(deposit => deposit.Status == PaymentStatus.Completed)
            .ToListAsync(cancellationToken);

        var totalInflow = completedDeposits.Sum(deposit => deposit.Amount);

        var paymentMethodRaw = completedDeposits
            .GroupBy(deposit => deposit.PaymentMethod)
            .Select(group => new
            {
                PaymentMethod = group.Key,
                Amount = group.Sum(item => item.Amount)
            })
            .OrderByDescending(item => item.Amount)
            .ToList();

        var refundOutflow = await _context.WalletTransactions
            .Where(transaction => transaction.TransactionType == TransactionType.Refund)
            .SumAsync(transaction => (decimal?)transaction.Amount, cancellationToken) ?? 0m;

        var withdrawalOutflow = await _context.WalletTransactions
            .Where(transaction => transaction.TransactionType == TransactionType.Withdrawal)
            .SumAsync(transaction => (decimal?)transaction.Amount, cancellationToken) ?? 0m;

        var totalOutflow = refundOutflow + withdrawalOutflow;

        var activeContractRatesByOrg = await (from contract in _context.Contracts
                                              join user in _context.Users on contract.UserId equals user.Id
                                              where contract.Status == ContractStatus.Active && user.OrganizationId != null
                                              group contract by user.OrganizationId!.Value
            into grouped
                                              select new
                                              {
                                                  OrganisationId = grouped.Key,
                                                  CommissionRate = grouped
                                                      .OrderByDescending(item => item.CreatedAt)
                                                      .Select(item => item.PlatformCommissionRate)
                                                      .FirstOrDefault()
                                              })
            .ToDictionaryAsync(item => item.OrganisationId, item => item.CommissionRate, cancellationToken);

        var completedRevenueByOrg = await _context.ConsultationSessions
            .Where(session => session.Status == SessionStatus.Completed && session.OrganisationId != null)
            .GroupBy(session => session.OrganisationId!.Value)
            .Select(group => new
            {
                OrganisationId = group.Key,
                Revenue = group.Sum(item => item.Price)
            })
            .ToListAsync(cancellationToken);

        var estimatedCommission = completedRevenueByOrg.Sum(item =>
        {
            if (!activeContractRatesByOrg.TryGetValue(item.OrganisationId, out var rate))
            {
                return 0m;
            }

            return item.Revenue * rate;
        });

        var paymentMethodBreakdown = paymentMethodRaw.Select(item => new PaymentMethodCashflowDto
        {
            PaymentMethod = item.PaymentMethod.ToString(),
            Amount = item.Amount,
            Percentage = totalInflow <= 0m ? 0m : Math.Round(item.Amount / totalInflow * 100m, 1)
        }).ToList();

        return new DashboardMetricsDto
        {
            TotalInflow = totalInflow,
            TotalOutflow = totalOutflow,
            RefundOutflow = refundOutflow,
            NetCashflow = totalInflow - totalOutflow,
            EstimatedCommission = Math.Round(estimatedCommission, 0),
            PaymentMethodBreakdown = paymentMethodBreakdown
        };
    }

    public async Task<PagedResult<RecentScreeningDto>> GetRecentScreeningsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = from screening in _context.AiScreenings.AsNoTracking()
                    join patient in _context.Patients.AsNoTracking() on screening.PatientId equals patient.Id
                    join user in _context.Users.AsNoTracking() on patient.UserId equals user.Id
                    join result in _context.ScreeningResults.AsNoTracking() on screening.Id equals result.AiScreeningId into resultJoin
                    from result in resultJoin.DefaultIfEmpty()
                    select new
                    {
                        screening.Id,
                        screening.CreatedAt,
                        screening.ProcessedAt,
                        PatientId = patient.Id,
                        PatientName = user.FullName,
                        RiskLevel = result != null ? result.RiskLevel.ToString() : null,
                        IsCritical = result != null && (result.RiskLevel == RiskLevel.High || result.RiskLevel == RiskLevel.Critical)
                    };

        var totalCount = await query.CountAsync(cancellationToken);
        var records = await query
            .OrderByDescending(item => item.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = records.Select(item => new RecentScreeningDto
        {
            Id = item.Id,
            ScreeningCode = $"SCR-{item.CreatedAt:yyyyMMdd}-{item.Id.ToString().Substring(0, 6).ToUpperInvariant()}",
            PatientId = item.PatientId,
            PatientName = item.PatientName,
            ClinicId = null,
            ClinicName = null,
            Status = item.ProcessedAt.HasValue ? "Completed" : "Analyzing",
            RiskLevel = item.RiskLevel,
            IsCritical = item.IsCritical,
            CreatedAt = item.CreatedAt,
            CompletedAt = item.ProcessedAt
        }).ToList();

        return new PagedResult<RecentScreeningDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<ScreeningVolumeTrendsDto> GetScreeningVolumeTrendsAsync(string timeRange, int periods, CancellationToken cancellationToken = default)
    {
        var normalizedTimeRange = string.Equals(timeRange, "weekly", StringComparison.OrdinalIgnoreCase)
            ? "weekly"
            : "monthly";

        var dataPoints = new List<VolumeTrendDataPoint>();
        if (normalizedTimeRange == "weekly")
        {
            var start = DateTime.UtcNow.Date.AddDays(-7 * (periods - 1));
            var screeningDates = await _context.AiScreenings
                .Where(s => s.CreatedAt >= start)
                .Select(s => s.CreatedAt)
                .ToListAsync(cancellationToken);

            dataPoints.AddRange(screeningDates
                .GroupBy(date =>
                {
                    var offset = ((int)date.DayOfWeek + 6) % 7;
                    return date.Date.AddDays(-offset);
                })
                .OrderBy(group => group.Key)
                .Select(group => new VolumeTrendDataPoint
                {
                    Date = group.Key,
                    Label = group.Key.ToString("dd MMM"),
                    Count = group.Count()
                }));
        }
        else
        {
            var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(periods - 1));
            var screenings = await _context.AiScreenings
                .Where(s => s.CreatedAt >= start)
                .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                .Select(group => new
                {
                    group.Key.Year,
                    group.Key.Month,
                    Count = group.Count()
                })
                .OrderBy(item => item.Year)
                .ThenBy(item => item.Month)
                .ToListAsync(cancellationToken);

            dataPoints.AddRange(screenings.Select(item =>
            {
                var date = new DateTime(item.Year, item.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                return new VolumeTrendDataPoint
                {
                    Date = date,
                    Label = date.ToString("MMM yyyy"),
                    Count = item.Count
                };
            }));
        }

        return new ScreeningVolumeTrendsDto
        {
            TimeRange = normalizedTimeRange,
            DataPoints = dataPoints,
            TotalScreenings = dataPoints.Sum(item => item.Count),
            AveragePerPeriod = dataPoints.Count == 0 ? 0 : Math.Round((decimal)dataPoints.Sum(item => item.Count) / dataPoints.Count, 1)
        };
    }

    public async Task<PopulationRiskAnalysisDto> GetPopulationRiskAnalysisAsync(CancellationToken cancellationToken = default)
    {
        var totalPatients = await _context.Patients.CountAsync(cancellationToken);
        var grouped = await _context.ScreeningResults
            .GroupBy(result => result.RiskLevel)
            .Select(group => new { group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        var totalResults = grouped.Sum(item => item.Count);
        return new PopulationRiskAnalysisDto
        {
            TotalPatients = totalPatients,
            RiskCategories = grouped
                .Where(item => item.Key != RiskLevel.None)
                .OrderBy(item => item.Key)
                .Select(item => new RiskCategoryDto
                {
                    RiskLevel = item.Key.ToString(),
                    Count = item.Count,
                    Percentage = totalResults == 0 ? 0 : Math.Round((decimal)item.Count / totalResults * 100m, 1)
                })
                .ToList()
        };
    }

    public Task<SystemHealthDto> GetSystemHealthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new SystemHealthDto
        {
            AllSystemsOperational = true,
            Components = new List<ComponentHealthDto>
            {
                new()
                {
                    ComponentName = "Database",
                    Status = "Connected",
                    IsHealthy = true,
                    LatencyMs = 25,
                    UptimePercentage = 100,
                    LastCheckedAt = DateTime.UtcNow
                },
                new()
                {
                    ComponentName = "AI Service",
                    Status = "Online",
                    IsHealthy = true,
                    LatencyMs = 120,
                    UptimePercentage = 100,
                    LastCheckedAt = DateTime.UtcNow
                },
                new()
                {
                    ComponentName = "Notifications",
                    Status = "Operational",
                    IsHealthy = true,
                    LatencyMs = 40,
                    UptimePercentage = 100,
                    LastCheckedAt = DateTime.UtcNow
                }
            }
        });
    }

    public async Task<OphthalmologistDashboardMetricsDto> GetOphthalmologistMetricsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var doctorId = await _context.Ophthalmologists
            .Where(o => o.UserId == userId)
            .Select(o => (Guid?)o.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!doctorId.HasValue)
        {
            return new OphthalmologistDashboardMetricsDto();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var pendingReviews = await _context.ConsultationSessions.CountAsync(
            session => session.OphthalmologistId == doctorId && session.Status == SessionStatus.Pending,
            cancellationToken);
        var urgentCases = await (from session in _context.ConsultationSessions
                                 join result in _context.ScreeningResults on session.AiScreeningId equals result.AiScreeningId
                                 where session.OphthalmologistId == doctorId &&
                                       (result.RiskLevel == RiskLevel.High || result.RiskLevel == RiskLevel.Critical)
                                 select session.Id)
            .Distinct()
            .CountAsync(cancellationToken);

          var urgentCaseList = await (from session in _context.ConsultationSessions
                            join result in _context.ScreeningResults on session.AiScreeningId equals result.AiScreeningId
                            join patient in _context.Patients on session.PatientId equals patient.Id
                            join user in _context.Users on patient.UserId equals user.Id
                            where session.OphthalmologistId == doctorId
                                && session.Status == SessionStatus.Pending
                                && (result.RiskLevel == RiskLevel.High || result.RiskLevel == RiskLevel.Critical)
                            select new OphthalmologistUrgentCaseDto
                            {
                                ConsultationSessionId = session.Id,
                                PatientId = patient.Id,
                                PatientName = user.FullName,
                                RiskLevel = result.RiskLevel.ToString(),
                                ConfidenceScore = result.ConfidenceScore,
                                AppointmentTime = session.AppointmentTime,
                                CreatedAt = session.CreatedAt
                            })
            .OrderByDescending(item => item.RiskLevel == RiskLevel.Critical.ToString())
            .ThenBy(item => item.AppointmentTime ?? DateTime.MaxValue)
            .ThenByDescending(item => item.CreatedAt)
            .Take(8)
            .ToListAsync(cancellationToken);
        var completedToday = await _context.Appointments.CountAsync(
            appointment => appointment.DoctorId == doctorId &&
                           appointment.Status == AppointmentStatus.Completed &&
                           appointment.CompletedAt.HasValue &&
                           appointment.CompletedAt.Value >= DateTime.UtcNow.Date,
            cancellationToken);
        var openSlotsToday = await (from slot in _context.AppointmentSlots
                                    join template in _context.ScheduleTemplates on slot.ScheduleTemplateId equals template.Id
                                    where template.OphthalId == doctorId && slot.Date == today && slot.Status == ScheduleStatus.Available
                                    select slot.Id)
            .CountAsync(cancellationToken);

        return new OphthalmologistDashboardMetricsDto
        {
            PendingReviews = pendingReviews,
            UrgentCases = urgentCases,
            CompletedToday = completedToday,
            OpenSlotsToday = openSlotsToday,
            UrgentCaseList = urgentCaseList
        };
    }

    public async Task<OrganisationDashboardMetricsDto> GetOrganisationMetricsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var organisationId = await _context.Users
            .Where(user => user.Id == userId)
            .Select(user => user.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (!organisationId.HasValue)
        {
            return new OrganisationDashboardMetricsDto();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var appointmentsQuery = _context.Appointments.Where(appointment => appointment.OrganisationId == organisationId);

        var totalAppointments = await appointmentsQuery.CountAsync(cancellationToken);
        var pendingCount = await appointmentsQuery.CountAsync(
            appointment => appointment.Status == AppointmentStatus.Pending,
            cancellationToken);
        var confirmedCount = await appointmentsQuery.CountAsync(
            appointment => appointment.Status == AppointmentStatus.Confirmed,
            cancellationToken);
        var completedCount = await appointmentsQuery.CountAsync(
            appointment => appointment.Status == AppointmentStatus.Completed,
            cancellationToken);
        var cancelledCount = await appointmentsQuery.CountAsync(
            appointment => appointment.Status == AppointmentStatus.Cancelled,
            cancellationToken);
        var noShowCount = await appointmentsQuery.CountAsync(
            appointment => appointment.Status == AppointmentStatus.NoShow,
            cancellationToken);

        var todayCapacity = await (from slot in _context.AppointmentSlots
                                   join template in _context.ScheduleTemplates on slot.ScheduleTemplateId equals template.Id
                                   where template.OrgId == organisationId && slot.Date == today
                                   select new { slot.BookedCount, slot.MaxCapacity })
            .ToListAsync(cancellationToken);

        var totalBooked = todayCapacity.Sum(item => item.BookedCount);
        var totalCapacity = todayCapacity.Sum(item => item.MaxCapacity);
        var utilizationRate = totalCapacity <= 0
            ? 0m
            : Math.Round((decimal)totalBooked / totalCapacity * 100m, 1);

        var quota = await _aiQuotaService.GetQuotaAsync(userId, "OrgAdmin", cancellationToken);

        return new OrganisationDashboardMetricsDto
        {
            UtilizationRatePercent = utilizationRate,
            RemainingAiQuota = quota.RemainingQuota,
            TotalAppointments = totalAppointments,
            AppointmentStatus = new OrganisationAppointmentStatusBreakdownDto
            {
                Pending = pendingCount,
                Confirmed = confirmedCount,
                Completed = completedCount,
                Cancelled = cancelledCount,
                NoShow = noShowCount
            }
        };
    }

    public async Task<PatientDashboardMetricsDto> GetPatientMetricsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (patient == null)
        {
            return new PatientDashboardMetricsDto();
        }

        var quota = await _aiQuotaService.GetQuotaAsync(userId, "Patient", cancellationToken);

        return new PatientDashboardMetricsDto
        {
            CompletedScreenings = await _context.AiScreenings.CountAsync(screening => screening.PatientId == patient.Id && screening.ProcessedAt.HasValue, cancellationToken),
            TotalReports = await (from screening in _context.AiScreenings
                                  join report in _context.ScreeningResults on screening.Id equals report.AiScreeningId
                                  where screening.PatientId == patient.Id
                                  select report.Id).CountAsync(cancellationToken),
            UpcomingAppointments = await _context.Appointments.CountAsync(appointment => appointment.PatientId == patient.Id && (appointment.Status == AppointmentStatus.Pending || appointment.Status == AppointmentStatus.Confirmed), cancellationToken),
            RemainingQuota = quota.RemainingQuota
        };
    }
}