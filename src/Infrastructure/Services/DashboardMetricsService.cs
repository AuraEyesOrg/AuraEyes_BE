using Application.Common.Constants;
using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using Application.Ophthalmologists.Queries.GetReviewQueue;
using Application.Patients.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkload;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkloads;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorStatus;
using Application.SystemAdmin.Dashboard.Queries.GetLiveQueue;
using Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;
using Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;
using Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;
using Application.SystemAdmin.Dashboard.Queries.GetSystemHealth;
using Application.SystemAdmin.Dashboard.Queries.GetSlotUtilization;
using Application.SystemAdmin.Dashboard.Queries.GetTodaySummary;
using Application.SystemAdmin.Dashboard.Queries.GetTransactionStats;
using Domain.Enums;
using Domain.Entities.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Services;

public class DashboardMetricsService : IDashboardMetricsService
{
    private const string FullTimeRequiredHoursWeekSettingKey = "FULLTIME_REQUIRED_HOURS_WEEK";
    private const string FullTimeRequiredHoursMonthSettingKey = "FULLTIME_REQUIRED_HOURS_MONTH";

    private readonly ApplicationDbContext _context;
    private readonly IBetterStackHeartbeatService _betterStackHeartbeatService;

    public DashboardMetricsService(
        ApplicationDbContext context,
        IBetterStackHeartbeatService betterStackHeartbeatService)
    {
        _context = context;
        _betterStackHeartbeatService = betterStackHeartbeatService;
    }

    public async Task<DashboardMetricsDto> GetSystemAdminMetricsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var (doctorMetrics, patientMetrics) = await GetUserStatisticsAsync(now, cancellationToken);

        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextYearStart = yearStart.AddYears(1);

        var revenueData = InitializeRevenueData(now);
        var (monthlyNewDoctorCounts, monthlyNewPatientCounts) = await GetMonthlyGrowthCountsAsync(yearStart, nextYearStart, cancellationToken);

        return new DashboardMetricsDto
        {
            Doctors = doctorMetrics,
            Patients = patientMetrics,
            Organisations = CreateEmptyGrowthMetric(),
            PaymentMethodBreakdown = new List<PaymentMethodRevenueDto>(),
            MonthlyRevenue = revenueData.MonthlyRevenue,
            DailyRevenue = revenueData.DailyRevenue,
            MonthlyPlatformCommission = revenueData.MonthlyCommission,
            DailyPlatformCommission = revenueData.DailyCommission,
            TotalDepositRevenueYear = 0m,
            TotalPlatformCommissionYear = 0m,
            MonthlyNewDoctorCounts = monthlyNewDoctorCounts,
            MonthlyNewOrganisationCounts = Enumerable.Range(1, 12).Select(_ => 0).ToList(),
            MonthlyNewPatientCounts = monthlyNewPatientCounts,
            PendingActions = new DashboardPendingActionsDto { PendingOphthalmologistVerifications = 0, PendingOrganisationOnboarding = 0 },
            SystemStatus = await GetSystemStatusAsync(cancellationToken),
            BetterStack = GetBetterStackStatus(),
            TopDoctorsByConsultationRevenue = new List<TopPerformerDoctorDto>(),
            TopOrganisationsByRating = new List<TopPerformerOrganisationDto>()
        };
    }

    private static RevenueData InitializeRevenueData(DateTime now)
    {
        var sevenDaysStart = now.Date.AddDays(-6);
        var months = Enumerable.Range(1, 12).Select(m => new MonthlyRevenuePointDto 
        { 
            Month = m, 
            Label = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(m), 
            Revenue = 0m 
        }).ToList();

        var days = Enumerable.Range(0, 7).Select(offset => sevenDaysStart.AddDays(offset)).Select(date => new DailyRevenuePointDto
        {
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
            Label = date.ToString("dd MMM", CultureInfo.InvariantCulture),
            Revenue = 0m
        }).ToList();

        return new RevenueData(
            months, 
            days, 
            months.Select(m => new MonthlyRevenuePointDto { Month = m.Month, Label = m.Label, Revenue = 0m }).ToList(),
            days.Select(d => new DailyRevenuePointDto { Date = d.Date, Label = d.Label, Revenue = 0m }).ToList());
    }

    private async Task<DashboardSystemStatusDto> GetSystemStatusAsync(CancellationToken cancellationToken)
    {
        var liveConsultations = await _context.ConsultationSessions
            .CountAsync(s => s.ChatStatus == ChatStatus.Open && s.Status != SessionStatus.Completed && s.Status != SessionStatus.Cancelled, cancellationToken);

        return new DashboardSystemStatusDto
        {
            LiveConsultationSessions = liveConsultations,
            ApiHealthy = true,
            DatabaseHealthy = await _context.Database.CanConnectAsync(cancellationToken)
        };
    }

    private DashboardBetterStackDto GetBetterStackStatus()
    {
        var monitorDescriptors = _betterStackHeartbeatService.GetMonitorDescriptors();
        return new DashboardBetterStackDto
        {
            Enabled = monitorDescriptors.Any(item => item.Configured),
            EmbedUrl = _betterStackHeartbeatService.GetEmbedUrl(),
            Monitors = monitorDescriptors.Select(item => new DashboardBackgroundMonitorDto
            {
                Key = item.Key,
                Name = item.DisplayName,
                Category = item.Category,
                Configured = item.Configured
            }).ToList()
        };
    }

    private static UserGrowthMetricDto CreateEmptyGrowthMetric() => new() { Total = 0, CurrentMonth = 0, PreviousMonth = 0, GrowthPercentage = 0 };

    private sealed record RevenueData(
        List<MonthlyRevenuePointDto> MonthlyRevenue, 
        List<DailyRevenuePointDto> DailyRevenue, 
        List<MonthlyRevenuePointDto> MonthlyCommission, 
        List<DailyRevenuePointDto> DailyCommission);


    private async Task<(UserGrowthMetricDto Doctors, UserGrowthMetricDto Patients)> GetUserStatisticsAsync(
        DateTime now, CancellationToken cancellationToken)
    {
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = currentMonthStart.AddMonths(1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);

        // Doctors
        var doctorTotalCount = await _context.Ophthalmologists.AsNoTracking().CountAsync(cancellationToken);
        var doctorCurrentMonthCount = await _context.Ophthalmologists.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= currentMonthStart && item.CreatedAt < nextMonthStart, cancellationToken);
        var doctorPreviousMonthCount = await _context.Ophthalmologists.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= previousMonthStart && item.CreatedAt < currentMonthStart, cancellationToken);

        // Patients
        var patientTotalCount = await _context.Patients.AsNoTracking().CountAsync(cancellationToken);
        var patientCurrentMonthCount = await _context.Patients.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= currentMonthStart && item.CreatedAt < nextMonthStart, cancellationToken);
        var patientPreviousMonthCount = await _context.Patients.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= previousMonthStart && item.CreatedAt < currentMonthStart, cancellationToken);

        return (
            new UserGrowthMetricDto
            {
                Total = doctorTotalCount,
                CurrentMonth = doctorCurrentMonthCount,
                PreviousMonth = doctorPreviousMonthCount,
                GrowthPercentage = CalculateGrowthPercentage(doctorCurrentMonthCount, doctorPreviousMonthCount)
            },
            new UserGrowthMetricDto
            {
                Total = patientTotalCount,
                CurrentMonth = patientCurrentMonthCount,
                PreviousMonth = patientPreviousMonthCount,
                GrowthPercentage = CalculateGrowthPercentage(patientCurrentMonthCount, patientPreviousMonthCount)
            }
        );
    }

    private async Task<(List<int> Doctors, List<int> Patients)> GetMonthlyGrowthCountsAsync(
        DateTime yearStart, DateTime nextYearStart, CancellationToken cancellationToken)
    {
        var newDoctorsByMonth = await _context.Ophthalmologists.AsNoTracking()
            .Where(o => o.CreatedAt >= yearStart && o.CreatedAt < nextYearStart)
            .GroupBy(o => o.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var newPatientsByMonth = await _context.Patients.AsNoTracking()
            .Where(p => p.CreatedAt >= yearStart && p.CreatedAt < nextYearStart)
            .GroupBy(p => p.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var doctorMonthMap = newDoctorsByMonth.ToDictionary(x => x.Month, x => x.Count);
        var patientMonthMap = newPatientsByMonth.ToDictionary(x => x.Month, x => x.Count);

        return (
            Enumerable.Range(1, 12).Select(m => doctorMonthMap.GetValueOrDefault(m, 0)).ToList(),
            Enumerable.Range(1, 12).Select(m => patientMonthMap.GetValueOrDefault(m, 0)).ToList()
        );
    }

    private static decimal CalculateGrowthPercentage(int currentMonthCount, int previousMonthCount)
    {
        if (previousMonthCount == 0)
        {
            return currentMonthCount == 0 ? 0m : 100m;
        }

        return Math.Round(((decimal)(currentMonthCount - previousMonthCount) / previousMonthCount) * 100m, 1);
    }

    public async Task<PagedResult<RecentScreeningDto>> GetRecentScreeningsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var visibleScreeningsQuery =
            from screening in _context.AiScreenings.AsNoTracking()
            join patient in _context.Patients.AsNoTracking() on screening.PatientId equals patient.Id
            join user in _context.Users.AsNoTracking() on patient.UserId equals user.Id
            select new
            {
                screening.Id,
                screening.PatientId,
                screening.CreatedAt,
                screening.ProcessedAt,
                PatientName = user.FullName
            };

        var totalCount = await visibleScreeningsQuery.CountAsync(cancellationToken);

        var screeningRiskQuery = _context.ScreeningResults.AsNoTracking()
            .GroupBy(result => result.AiScreeningId)
            .Select(group => new
            {
                AiScreeningId = group.Key,
                RiskLevel = group.Select(item => (RiskLevel?)item.RiskLevel).FirstOrDefault()
            });

        var pageRows = await (
            from screening in visibleScreeningsQuery
            join risk in screeningRiskQuery on screening.Id equals risk.AiScreeningId into riskJoin
            from risk in riskJoin.DefaultIfEmpty()
            orderby screening.CreatedAt descending
            select new
            {
                screening.Id,
                screening.PatientId,
                screening.PatientName,
                screening.CreatedAt,
                screening.ProcessedAt,
                RiskLevel = risk != null ? risk.RiskLevel : null
            })
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = pageRows.Select(MapToRecentScreeningDto).ToList();

        return new PagedResult<RecentScreeningDto>(items, totalCount, pageNumber, pageSize);
    }

    private static RecentScreeningDto MapToRecentScreeningDto(dynamic item)
    {
        return new RecentScreeningDto
        {
            Id = item.Id,
            ScreeningCode = $"SCR-{item.CreatedAt:yyyyMMdd}-{item.Id.ToString().Substring(0, 6).ToUpperInvariant()}",
            PatientId = item.PatientId,
            PatientName = item.PatientName,
            ClinicId = null,
            ClinicName = null,
            Status = item.ProcessedAt.HasValue ? "Completed" : "Analyzing",
            RiskLevel = item.RiskLevel?.ToString(),
            IsCritical = item.RiskLevel == RiskLevel.High || item.RiskLevel == RiskLevel.Critical,
            CreatedAt = item.CreatedAt,
            CompletedAt = item.ProcessedAt
        };
    }

    public async Task<ScreeningVolumeTrendsDto> GetScreeningVolumeTrendsAsync(string timeRange, int periods, CancellationToken cancellationToken = default)
    {
        var normalizedTimeRange = string.Equals(timeRange, "weekly", StringComparison.OrdinalIgnoreCase)
            ? "weekly"
            : "monthly";

        var dataPoints = normalizedTimeRange == "weekly"
            ? await GetWeeklyTrendDataPointsAsync(periods, cancellationToken)
            : await GetMonthlyTrendDataPointsAsync(periods, cancellationToken);

        return new ScreeningVolumeTrendsDto
        {
            TimeRange = normalizedTimeRange,
            DataPoints = dataPoints,
            TotalScreenings = dataPoints.Sum(item => item.Count),
            AveragePerPeriod = dataPoints.Count == 0 ? 0 : Math.Round((decimal)dataPoints.Sum(item => item.Count) / dataPoints.Count, 1)
        };
    }

    private async Task<List<VolumeTrendDataPoint>> GetWeeklyTrendDataPointsAsync(int periods, CancellationToken cancellationToken)
    {
        var start = DateTime.UtcNow.Date.AddDays(-7 * (periods - 1));
        var screeningDates = await _context.AiScreenings
            .Where(s => s.CreatedAt >= start)
            .Select(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        return screeningDates
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
            })
            .ToList();
    }

    private async Task<List<VolumeTrendDataPoint>> GetMonthlyTrendDataPointsAsync(int periods, CancellationToken cancellationToken)
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

        return screenings.Select(item =>
        {
            var date = new DateTime(item.Year, item.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return new VolumeTrendDataPoint
            {
                Date = date,
                Label = date.ToString("MMM yyyy"),
                Count = item.Count
            };
        }).ToList();
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

    public async Task<DoctorWorkloadDto?> GetDoctorWorkloadAsync(
        Guid doctorId,
        WorkloadPeriodType periodType,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var doctor = await (from ophthal in _context.Ophthalmologists.AsNoTracking()
                            join user in _context.Users.AsNoTracking() on ophthal.UserId equals user.Id
                            where ophthal.Id == doctorId && !user.IsDeleted
                            select new DoctorProjection
                            {
                                Id = ophthal.Id,
                                Name = user.FullName,
                                Email = user.Email,
                                EmploymentType = ophthal.EmploymentType
                            })
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return null;

        var anchorDate = ResolveAnchorDate(date);
        var periodBounds = GetPeriodBounds(periodType, anchorDate);

        var intervals = await QueryCompletedSessionIntervalsAsync(
            new[] { doctor.Id },
            periodBounds.StartUtc,
            periodBounds.EndUtcExclusive,
            cancellationToken);

        var actualHours = CalculateMergedHours(intervals, periodBounds.StartUtc, periodBounds.EndUtcExclusive);
        var requiredHours = await GetRequiredHoursAsync(doctor.EmploymentType, periodType, cancellationToken);
        var completionRate = CalculateCompletionRate(actualHours, requiredHours);
        var status = actualHours >= requiredHours ? "OK" : "UNDER";

        return new DoctorWorkloadDto
        {
            DoctorId = doctor.Id,
            PeriodType = ToApiPeriodType(periodType),
            PeriodStart = periodBounds.StartUtc,
            PeriodEnd = periodBounds.EndUtcInclusive,
            RequiredHours = requiredHours,
            ActualHours = actualHours,
            CompletionRate = completionRate,
            Status = status
        };
    }

    public async Task<PagedResult<DoctorWorkloadListItemDto>> GetDoctorWorkloadsAsync(
        WorkloadPeriodType periodType,
        DateOnly date,
        string? searchTerm,
        OphthalmologistEmploymentType? employmentType,
        string? status,
        bool warningOnly,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedStatus = NormalizeStatusFilter(status);
        var anchorDate = ResolveAnchorDate(date);
        var periodBounds = GetPeriodBounds(periodType, anchorDate);

        var doctorsQuery = from ophthal in _context.Ophthalmologists.AsNoTracking()
                           join user in _context.Users.AsNoTracking() on ophthal.UserId equals user.Id
                           where !user.IsDeleted
                           select new DoctorProjection
                           {
                               Id = ophthal.Id,
                               Name = user.FullName,
                               Email = user.Email,
                               EmploymentType = ophthal.EmploymentType
                           };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearch = $"%{searchTerm.Trim()}%";
            doctorsQuery = doctorsQuery.Where(x => EF.Functions.ILike(x.Name, normalizedSearch) || (x.Email != null && EF.Functions.ILike(x.Email, normalizedSearch)));
        }

        if (employmentType.HasValue) doctorsQuery = doctorsQuery.Where(x => x.EmploymentType == employmentType.Value);

        var doctors = await doctorsQuery.ToListAsync(cancellationToken);
        if (doctors.Count == 0) return new PagedResult<DoctorWorkloadListItemDto>(new List<DoctorWorkloadListItemDto>(), 0, pageNumber, pageSize);

        var intervals = await QueryCompletedSessionIntervalsAsync(doctors.Select(x => x.Id).ToList(), periodBounds.StartUtc, periodBounds.EndUtcExclusive, cancellationToken);
        var intervalsByDoctor = intervals.GroupBy(x => x.DoctorId).ToDictionary(g => g.Key, g => g.ToList());
        var requiredHoursByType = await GetRequiredHoursByEmploymentTypeAsync(periodType, cancellationToken);

        var computed = new List<DoctorWorkloadListItemDto>(doctors.Count);
        foreach (var doctor in doctors)
        {
            var item = ComputeDoctorWorkloadItem(doctor, intervalsByDoctor.GetValueOrDefault(doctor.Id), requiredHoursByType.GetValueOrDefault(doctor.EmploymentType, 0m), periodType, periodBounds);
            if (item == null) continue;
            if (normalizedStatus != null && !string.Equals(item.Status, normalizedStatus, StringComparison.Ordinal)) continue;
            if (warningOnly && !item.WarningFlag) continue;

            computed.Add(item);
        }

        var ordered = computed.OrderByDescending(item => item.WarningFlag).ThenBy(item => item.CompletionRate).ThenBy(item => item.DoctorName).ToList();
        return new PagedResult<DoctorWorkloadListItemDto>(ordered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), ordered.Count, pageNumber, pageSize);
    }

    private DoctorWorkloadListItemDto ComputeDoctorWorkloadItem(
        DoctorProjection doctor,
        List<SessionIntervalProjection>? doctorIntervals,
        decimal requiredHours,
        WorkloadPeriodType periodType,
        (DateTime StartUtc, DateTime EndUtcInclusive, DateTime EndUtcExclusive) periodBounds)
    {
        var actualHours = CalculateMergedHours(doctorIntervals ?? new List<SessionIntervalProjection>(), periodBounds.StartUtc, periodBounds.EndUtcExclusive);
        var completionRate = CalculateCompletionRate(actualHours, requiredHours);
        var workloadStatus = actualHours >= requiredHours ? "OK" : "UNDER";
        var warningFlag = completionRate < 0.8m;

        return new DoctorWorkloadListItemDto
        {
            DoctorId = doctor.Id,
            DoctorName = doctor.Name,
            Email = doctor.Email,
            EmploymentType = ToApiEmploymentType(doctor.EmploymentType),
            PeriodType = ToApiPeriodType(periodType),
            PeriodStart = periodBounds.StartUtc,
            PeriodEnd = periodBounds.EndUtcInclusive,
            RequiredHours = requiredHours,
            ActualHours = actualHours,
            CompletionRate = completionRate,
            Status = workloadStatus,
            WarningFlag = warningFlag
        };
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

        var urgentCasesRaw = await (from session in _context.ConsultationSessions
                                    join result in _context.ScreeningResults on session.AiScreeningId equals result.AiScreeningId
                                    join patient in _context.Patients on session.PatientId equals patient.Id
                                    join user in _context.Users on patient.UserId equals user.Id
                                    where session.OphthalmologistId == doctorId
                                        && session.Status == SessionStatus.Pending
                                        && (result.RiskLevel == RiskLevel.High || result.RiskLevel == RiskLevel.Critical)
                                    select new
                                    {
                                        ConsultationSessionId = session.Id,
                                        PatientId = patient.Id,
                                        PatientName = user.FullName,
                                        result.RiskLevel,
                                        result.ConfidenceScore,
                                        AppointmentTime = session.AppointmentTime,
                                        CreatedAt = session.CreatedAt
                                    })
            .OrderByDescending(item => item.RiskLevel == RiskLevel.Critical)
            .ThenBy(item => item.AppointmentTime ?? DateTime.MaxValue)
            .ThenByDescending(item => item.CreatedAt)
            .Take(8)
            .ToListAsync(cancellationToken);

        var urgentCaseList = urgentCasesRaw
            .Select(item => new OphthalmologistUrgentCaseDto
            {
                ConsultationSessionId = item.ConsultationSessionId,
                PatientId = item.PatientId,
                PatientName = item.PatientName,
                RiskLevel = item.RiskLevel.ToString(),
                ConfidenceScore = item.ConfidenceScore,
                AppointmentTime = item.AppointmentTime,
                CreatedAt = item.CreatedAt
            })
            .ToList();
        var completedToday = await _context.ConsultationSessions.CountAsync(
            session => session.OphthalmologistId == doctorId &&
                           session.Status == SessionStatus.Completed &&
                           session.EndTime.HasValue &&
                           session.EndTime.Value >= DateTime.UtcNow.Date,
            cancellationToken);

        return new OphthalmologistDashboardMetricsDto
        {
            PendingReviews = pendingReviews,
            UrgentCases = urgentCases,
            CompletedToday = completedToday,
            OpenSlotsToday = 0,
            UrgentCaseList = urgentCaseList
        };
    }

    public async Task<PatientDashboardMetricsDto> GetPatientMetricsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patientId = await _context.Patients
            .Where(p => p.UserId == userId)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!patientId.HasValue)
            return new PatientDashboardMetricsDto();

        var screeningsCount = await _context.AiScreenings.CountAsync(s => s.PatientId == patientId, cancellationToken);
        var consultationsCount = await _context.ConsultationSessions.CountAsync(s => s.PatientId == patientId, cancellationToken);
        
        var nextAppointment = await _context.Appointments
            .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Confirmed)
            .OrderBy(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .Select(a => new { a.AppointmentSlot!.Date, a.AppointmentSlot!.StartTime })
            .FirstOrDefaultAsync(cancellationToken);

        return new PatientDashboardMetricsDto
        {
            CompletedScreenings = screeningsCount,
            TotalReports = consultationsCount,
            UpcomingAppointments = nextAppointment != null ? 1 : 0
        };
    }

    public async Task<List<ReviewQueueItemDto>> GetReviewQueueAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var doctorId = await _context.Ophthalmologists
            .Where(o => o.UserId == userId)
            .Select(o => (Guid?)o.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!doctorId.HasValue)
            return new List<ReviewQueueItemDto>();

        var rawItems = await (
            from session in _context.ConsultationSessions
            join patient in _context.Patients on session.PatientId equals patient.Id
            join user in _context.Users on patient.UserId equals user.Id
            join screening in _context.AiScreenings on session.AiScreeningId equals screening.Id into screeningJoin
            from screening in screeningJoin.DefaultIfEmpty()
            join result in _context.ScreeningResults on screening.Id equals result.AiScreeningId into resultJoin
            from result in resultJoin.DefaultIfEmpty()
            where session.OphthalmologistId == doctorId
                  && (session.Status == SessionStatus.Pending || session.Status == SessionStatus.Confirmed)
            select new
            {
                SessionId = session.Id,
                ScreeningId = screening != null ? screening.Id : Guid.Empty,
                session.PatientId,
                PatientName = user.FullName,
                RiskLevel = result != null ? result.RiskLevel : RiskLevel.None,
                ConfidenceScore = result != null ? result.ConfidenceScore : 0m,
                AiSummary = result != null ? result.Summary : null,
                session.AppointmentTime,
                session.CreatedAt,
                session.Status
            })
            .OrderByDescending(item => item.RiskLevel == RiskLevel.Critical)
            .ThenBy(item => item.AppointmentTime ?? DateTime.MaxValue)
            .ThenByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return rawItems.Select(item => new ReviewQueueItemDto
        {
            ConsultationSessionId = item.SessionId,
            ScreeningId = item.ScreeningId,
            PatientId = item.PatientId,
            PatientName = item.PatientName,
            RiskLevel = item.RiskLevel.ToString(),
            ConfidenceScore = item.ConfidenceScore,
            AiSummary = item.AiSummary,
            AppointmentTime = item.AppointmentTime,
            CreatedAt = item.CreatedAt,
            ReviewStatus = item.Status.ToString()
        }).ToList();
    }

    public async Task<TodaySummaryDto> GetTodaySummaryAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, VietnamTimeZoneResolver.TimeZone);
        var today = DateOnly.FromDateTime(vietnamNow);
        var yesterday = today.AddDays(-1);
        
        var currentMonthStart = today.AddDays(-today.Day + 1);
        var lastMonthStart = currentMonthStart.AddMonths(-1);
        var lastMonthEnd = currentMonthStart.AddDays(-1);

        var currentYearStart = new DateOnly(today.Year, 1, 1);
        var lastYearStart = currentYearStart.AddYears(-1);
        var lastYearEnd = currentYearStart.AddDays(-1);

        // Today stats
        var appointments = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date == today, cancellationToken);
        var checkedIn = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date == today && a.Status == AppointmentStatus.CheckedIn, cancellationToken);
        var completed = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date == today && a.Status == AppointmentStatus.Completed, cancellationToken);
        var noShow = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date == today && a.Status == AppointmentStatus.NoShow, cancellationToken);
        var todayRevenue = await _context.Appointments
            .Where(a => a.AppointmentSlot!.Date == today && a.Status == AppointmentStatus.Completed)
            .SumAsync(a => a.Price, cancellationToken);

        // Comparison stats for growth
        var yesterdayCount = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date == yesterday, cancellationToken);
        var yesterdayRevenue = await _context.Appointments
            .Where(a => a.AppointmentSlot!.Date == yesterday && a.Status == AppointmentStatus.Completed)
            .SumAsync(a => a.Price, cancellationToken);
        
        var thisMonthCount = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date >= currentMonthStart && a.AppointmentSlot!.Date <= today, cancellationToken);
        var lastMonthCount = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date >= lastMonthStart && a.AppointmentSlot!.Date <= lastMonthEnd, cancellationToken);

        var thisMonthRevenue = await _context.Appointments
            .Where(a => a.AppointmentSlot!.Date >= currentMonthStart && a.AppointmentSlot!.Date <= today && a.Status == AppointmentStatus.Completed)
            .SumAsync(a => a.Price, cancellationToken);
        var lastMonthRevenue = await _context.Appointments
            .Where(a => a.AppointmentSlot!.Date >= lastMonthStart && a.AppointmentSlot!.Date <= lastMonthEnd && a.Status == AppointmentStatus.Completed)
            .SumAsync(a => a.Price, cancellationToken);

        var thisYearCount = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date >= currentYearStart && a.AppointmentSlot!.Date <= today, cancellationToken);
        var lastYearCount = await _context.Appointments
            .CountAsync(a => a.AppointmentSlot!.Date >= lastYearStart && a.AppointmentSlot!.Date <= lastYearEnd, cancellationToken);

        var thisYearRevenue = await _context.Appointments
            .Where(a => a.AppointmentSlot!.Date >= currentYearStart && a.AppointmentSlot!.Date <= today && a.Status == AppointmentStatus.Completed)
            .SumAsync(a => a.Price, cancellationToken);
        var lastYearRevenue = await _context.Appointments
            .Where(a => a.AppointmentSlot!.Date >= lastYearStart && a.AppointmentSlot!.Date <= lastYearEnd && a.Status == AppointmentStatus.Completed)
            .SumAsync(a => a.Price, cancellationToken);

        return new TodaySummaryDto
        {
            TotalAppointments = appointments,
            MonthAppointments = thisMonthCount,
            YearAppointments = thisYearCount,
            CheckedInPatients = checkedIn,
            CompletedVisits = completed,
            NoShowCount = noShow,
            TodayRevenue = todayRevenue,
            MonthRevenue = thisMonthRevenue,
            YearRevenue = thisYearRevenue,
            GrowthPercentageDay = CalculateGrowthPercentage(appointments, yesterdayCount),
            GrowthPercentageMonth = CalculateGrowthPercentage(thisMonthCount, lastMonthCount),
            GrowthPercentageYear = CalculateGrowthPercentage(thisYearCount, lastYearCount),
            RevenueGrowthPercentageDay = CalculateGrowthPercentage((double)todayRevenue, (double)yesterdayRevenue),
            RevenueGrowthPercentageMonth = CalculateGrowthPercentage((double)thisMonthRevenue, (double)lastMonthRevenue),
            RevenueGrowthPercentageYear = CalculateGrowthPercentage((double)thisYearRevenue, (double)lastYearRevenue)
        };
    }

    private static decimal CalculateGrowthPercentage(double current, double previous)
    {
        if (previous == 0) return current > 0 ? 100 : 0;
        return (decimal)Math.Round((current - previous) / previous * 100, 2);
    }

    public async Task<SlotUtilizationDto> GetSlotUtilizationAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var slots = await _context.AppointmentSlots
            .Where(s => s.Date == today)
            .ToListAsync(cancellationToken);

        var totalCapacity = slots.Sum(s => s.MaxCapacity);
        var booked = slots.Sum(s => s.BookedCount);

        return new SlotUtilizationDto
        {
            TotalSlots = totalCapacity,
            BookedSlots = booked,
            UtilizationRate = totalCapacity == 0 ? 0 : (decimal)booked / totalCapacity * 100
        };
    }

    public async Task<IReadOnlyList<LiveQueueItemDto>> GetLiveQueueAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, VietnamTimeZoneResolver.TimeZone);
        var today = DateOnly.FromDateTime(vietnamNow);

        var queueItems = await (from a in _context.Appointments.AsNoTracking()
                               join p in _context.Patients.AsNoTracking() on a.PatientId equals p.Id
                               join u in _context.Users.AsNoTracking() on p.UserId equals u.Id
                               join d in _context.Ophthalmologists.AsNoTracking() on a.AppointmentSlot!.OphthalId equals d.Id into doctorJoin
                               from doctor in doctorJoin.DefaultIfEmpty()
                               join du in _context.Users.AsNoTracking() on doctor.UserId equals du.Id into doctorUserJoin
                               from doctorUser in doctorUserJoin.DefaultIfEmpty()
                               where a.AppointmentSlot!.Date == today &&
                                     (a.Status == AppointmentStatus.CheckedIn || a.Status == AppointmentStatus.InProgress)
                               orderby a.Status == AppointmentStatus.InProgress descending, a.UpdatedAt ascending
                               select new
                               {
                                   a.Id,
                                   u.FullName,
                                   a.Status,
                                   a.UpdatedAt,
                                   a.CreatedAt,
                                   DoctorName = doctorUser != null ? doctorUser.FullName : null
                               }).ToListAsync(cancellationToken);

        return queueItems.Select(item =>
        {
            var waitMinutes = (int)(nowUtc - (item.UpdatedAt ?? item.CreatedAt)).TotalMinutes;
            return new LiveQueueItemDto
            {
                VisitId = item.Id,
                PatientName = item.FullName,
                Status = item.Status.ToString(),
                AssignedDoctorName = item.DoctorName,
                WaitingTimeMinutes = Math.Max(0, waitMinutes),
                CheckedInAt = item.UpdatedAt
            };
        }).ToList();
    }

    public async Task<IReadOnlyList<DoctorStatusDto>> GetDoctorStatusAsync(CancellationToken cancellationToken = default)
    {
        var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZoneResolver.TimeZone);
        var today = DateOnly.FromDateTime(vietnamNow);

        var doctors = await (from d in _context.Ophthalmologists.AsNoTracking()
                            join u in _context.Users.AsNoTracking() on d.UserId equals u.Id
                            where !u.IsDeleted
                            select new
                            {
                                d.Id,
                                u.FullName
                            }).ToListAsync(cancellationToken);

        var doctorIds = doctors.Select(d => d.Id).ToList();

        // Get handled count today per doctor
        var handledCounts = await _context.Appointments.AsNoTracking()
            .Where(a => a.AppointmentSlot!.Date == today && a.Status == AppointmentStatus.Completed && a.AppointmentSlot.OphthalId.HasValue && doctorIds.Contains(a.AppointmentSlot.OphthalId.Value))
            .GroupBy(a => a.AppointmentSlot!.OphthalId)
            .Select(g => new { DoctorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DoctorId!.Value, x => x.Count, cancellationToken);

        // Get active load today per doctor
        var activeLoads = await _context.Appointments.AsNoTracking()
            .Where(a => a.AppointmentSlot!.Date == today && a.Status == AppointmentStatus.InProgress && a.AppointmentSlot.OphthalId.HasValue && doctorIds.Contains(a.AppointmentSlot.OphthalId.Value))
            .GroupBy(a => a.AppointmentSlot!.OphthalId)
            .Select(g => new { DoctorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DoctorId!.Value, x => x.Count, cancellationToken);

        return doctors.Select(d => new DoctorStatusDto
        {
            DoctorId = d.Id,
            DoctorName = d.FullName,
            CurrentStatus = activeLoads.GetValueOrDefault(d.Id, 0) > 0 ? "In consultation" : "Available",
            PatientsHandledToday = handledCounts.GetValueOrDefault(d.Id, 0),
            ActiveLoad = activeLoads.GetValueOrDefault(d.Id, 0)
        }).ToList();
    }

    public async Task<IReadOnlyList<TransactionStatsDto>> GetTransactionStatsAsync(string period, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(now, VietnamTimeZoneResolver.TimeZone);

        // Base query: completed payments with a paid date
        var paymentsQuery = _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.PaidAt != null);

        List<IGrouping<string, Domain.Entities.Financial.Payment>> groups;

        if (period == "monthly")
        {
            // Group by month for the current year
            var startOfYear = new DateTime(vietnamNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var startUtc = startOfYear.ToUniversalTime();
            var endUtc = startOfYear.AddYears(1).ToUniversalTime();

            var payments = await paymentsQuery
                .Where(p => p.PaidAt >= startUtc && p.PaidAt < endUtc)
                .ToListAsync(cancellationToken);

            groups = payments
                .GroupBy(p =>
                {
                    var local = TimeZoneInfo.ConvertTimeFromUtc(p.PaidAt!.Value, VietnamTimeZoneResolver.TimeZone);
                    return local.ToString("MMM yyyy", CultureInfo.InvariantCulture);
                })
                .ToList();
        }
        else if (period == "weekly")
        {
            // Group by ISO week for last 4 weeks
            var weeksBack = vietnamNow.AddDays(-27);
            var weeksBackUtc = weeksBack.ToUniversalTime();

            var payments = await paymentsQuery
                .Where(p => p.PaidAt >= weeksBackUtc)
                .ToListAsync(cancellationToken);

            groups = payments
                .GroupBy(p =>
                {
                    var local = TimeZoneInfo.ConvertTimeFromUtc(p.PaidAt!.Value, VietnamTimeZoneResolver.TimeZone);
                    var isoWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        local, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return $"Week {isoWeek}";
                })
                .ToList();
        }
        else // daily
        {
            // Group by day for last 7 days
            var daysBack = vietnamNow.AddDays(-6);
            var daysBackUtc = daysBack.ToUniversalTime();

            var payments = await paymentsQuery
                .Where(p => p.PaidAt >= daysBackUtc)
                .ToListAsync(cancellationToken);

            groups = payments
                .GroupBy(p =>
                {
                    var local = TimeZoneInfo.ConvertTimeFromUtc(p.PaidAt!.Value, VietnamTimeZoneResolver.TimeZone);
                    return local.ToString("MMM dd", CultureInfo.InvariantCulture);
                })
                .ToList();
        }

        return groups
            .Select(g => new TransactionStatsDto
            {
                Date = g.Key,
                Amount = g.Sum(p => p.Amount),
                Count = g.Count(),
            })
            .OrderBy(x => x.Date)
            .ToList();
    }

    // Helper methods for workload calculation
    private static WorkloadPeriodType ResolvePeriodType(string period) =>
        string.Equals(period, "monthly", StringComparison.OrdinalIgnoreCase)
            ? WorkloadPeriodType.Month
            : WorkloadPeriodType.Week;

    private static DateOnly ResolveAnchorDate(DateOnly? date) =>
        date ?? DateOnly.FromDateTime(DateTime.UtcNow);

    private static (DateTime StartUtc, DateTime EndUtcInclusive, DateTime EndUtcExclusive) GetPeriodBounds(WorkloadPeriodType type, DateOnly anchor)
    {
        if (type == WorkloadPeriodType.Week)
        {
            var offset = ((int)anchor.DayOfWeek + 6) % 7;
            var start = anchor.AddDays(-offset);
            var end = start.AddDays(6);
            return (
                start.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                end.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc),
                end.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        }
        else
        {
            var start = new DateOnly(anchor.Year, anchor.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return (
                start.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                end.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc),
                start.AddMonths(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        }
    }

    private async Task<List<SessionIntervalProjection>> QueryCompletedSessionIntervalsAsync(
        IList<Guid> doctorIds,
        DateTime startUtc,
        DateTime endUtcExclusive,
        CancellationToken cancellationToken)
    {
        return await _context.ConsultationSessions.AsNoTracking()
            .Where(s => s.OphthalmologistId != null
                        && doctorIds.Contains(s.OphthalmologistId.Value)
                        && s.Status == SessionStatus.Completed
                        && s.StartTime.HasValue
                        && s.EndTime.HasValue
                        && s.EndTime.Value >= startUtc
                        && s.StartTime.Value < endUtcExclusive)
            .Select(s => new SessionIntervalProjection
            {
                DoctorId = s.OphthalmologistId!.Value,
                Start = s.StartTime!.Value,
                End = s.EndTime!.Value
            })
            .ToListAsync(cancellationToken);
    }

    private static decimal CalculateMergedHours(List<SessionIntervalProjection> intervals, DateTime periodStart, DateTime periodEndExclusive)
    {
        if (intervals.Count == 0) return 0m;

        var totalSeconds = 0d;
        foreach (var interval in intervals)
        {
            var s = interval.Start < periodStart ? periodStart : interval.Start;
            var e = interval.End > periodEndExclusive ? periodEndExclusive : interval.End;

            if (e > s)
            {
                totalSeconds += (e - s).TotalSeconds;
            }
        }

        return Math.Round((decimal)totalSeconds / 3600m, 1);
    }

    private async Task<decimal> GetRequiredHoursAsync(OphthalmologistEmploymentType type, WorkloadPeriodType periodType, CancellationToken cancellationToken)
    {
        if (type == OphthalmologistEmploymentType.PartTime) return 0m;

        var key = periodType == WorkloadPeriodType.Week
            ? FullTimeRequiredHoursWeekSettingKey
            : FullTimeRequiredHoursMonthSettingKey;

        var setting = await _context.SystemSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        if (setting is not null && decimal.TryParse(setting.Value, out var val))
            return val;

        return periodType == WorkloadPeriodType.Week ? 40m : 160m;
    }

    private async Task<Dictionary<OphthalmologistEmploymentType, decimal>> GetRequiredHoursByEmploymentTypeAsync(
        WorkloadPeriodType periodType, CancellationToken cancellationToken)
    {
        var fullTimeHours = await GetRequiredHoursAsync(OphthalmologistEmploymentType.FullTime, periodType, cancellationToken);
        return new Dictionary<OphthalmologistEmploymentType, decimal>
        {
            [OphthalmologistEmploymentType.FullTime] = fullTimeHours,
            [OphthalmologistEmploymentType.PartTime] = 0m
        };
    }

    private static decimal CalculateCompletionRate(decimal actual, decimal required) =>
        required <= 0 ? 1m : Math.Min(Math.Round(actual / required, 2), 2m);

    private static string? NormalizeStatusFilter(string? status) =>
        status?.ToUpperInvariant() switch
        {
            "OK" => "OK",
            "UNDER" => "UNDER",
            _ => null
        };

    private static string ToApiPeriodType(WorkloadPeriodType type) =>
        type == WorkloadPeriodType.Week ? "weekly" : "monthly";

    private static string ToApiEmploymentType(OphthalmologistEmploymentType type) =>
        type == OphthalmologistEmploymentType.FullTime ? "FullTime" : "PartTime";

    private class DoctorProjection
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public OphthalmologistEmploymentType EmploymentType { get; set; }
    }

    private class SessionIntervalProjection
    {
        public Guid DoctorId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}