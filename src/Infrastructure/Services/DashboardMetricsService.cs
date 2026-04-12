using Application.AiQuota.Interfaces;
using Application.Common.Constants;
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
using System.Globalization;

namespace Infrastructure.Services;

public class DashboardMetricsService : IDashboardMetricsService
{
    private readonly ApplicationDbContext _context;
    private readonly IAiQuotaService _aiQuotaService;
    private readonly IBetterStackHeartbeatService _betterStackHeartbeatService;

    public DashboardMetricsService(
        ApplicationDbContext context,
        IAiQuotaService aiQuotaService,
        IBetterStackHeartbeatService betterStackHeartbeatService)
    {
        _context = context;
        _aiQuotaService = aiQuotaService;
        _betterStackHeartbeatService = betterStackHeartbeatService;
    }

    public async Task<DashboardMetricsDto> GetSystemAdminMetricsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var utcToday = now.Date;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var nextMonthStart = currentMonthStart.AddMonths(1);
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var sevenDaysAgo = utcToday.AddDays(-6);

        var doctorGrowth = await BuildGrowthMetricAsync(
            _context.Ophthalmologists.AsNoTracking().Select(x => x.CreatedAt),
            previousMonthStart,
            currentMonthStart,
            nextMonthStart,
            cancellationToken);

        var organisationGrowth = await BuildGrowthMetricAsync(
            _context.Organisations.AsNoTracking().Select(x => x.CreatedAt),
            previousMonthStart,
            currentMonthStart,
            nextMonthStart,
            cancellationToken);

        var patientGrowth = await BuildGrowthMetricAsync(
            _context.Patients.AsNoTracking().Select(x => x.CreatedAt),
            previousMonthStart,
            currentMonthStart,
            nextMonthStart,
            cancellationToken);

        var completedDepositsCurrentYear = await _context.DepositRequests
            .AsNoTracking()
            .Where(x => x.Status == PaymentStatus.Completed)
            .Where(x => x.CompletedAt.HasValue && x.CompletedAt.Value >= yearStart)
            .Select(x => new
            {
                x.Amount,
                x.PaymentMethod,
                CompletedAt = x.CompletedAt!.Value
            })
            .ToListAsync(cancellationToken);

        var totalDepositRevenueYear = completedDepositsCurrentYear.Sum(x => x.Amount);

        var paymentMethodBreakdown = completedDepositsCurrentYear
            .GroupBy(x => x.PaymentMethod)
            .Select(group =>
            {
                var amount = group.Sum(x => x.Amount);
                var percentage = totalDepositRevenueYear <= 0
                    ? 0m
                    : Math.Round(amount / totalDepositRevenueYear * 100m, 2);

                return new DashboardPaymentMethodRevenueDto
                {
                    PaymentMethod = group.Key.ToString(),
                    Amount = amount,
                    Percentage = percentage
                };
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        var monthlyRevenue = BuildMonthlyRevenuePoints(
            completedDepositsCurrentYear,
            now.Year,
            selector: x => x.CompletedAt,
            amountSelector: x => x.Amount);

        var dailyRevenue = BuildDailyRevenuePoints(
            completedDepositsCurrentYear,
            sevenDaysAgo,
            selector: x => x.CompletedAt,
            amountSelector: x => x.Amount);

        var platformCommissionRate = await GetPlatformCommissionRateAsync(cancellationToken);

        var completedConsultationsCurrentYear = await _context.ConsultationSessions
            .AsNoTracking()
            .Where(x => x.Status == SessionStatus.Completed)
            .Where(x => x.ClosedAt.HasValue && x.ClosedAt.Value >= yearStart)
            .Select(x => new
            {
                x.Id,
                x.OphthalmologistId,
                x.Price,
                ClosedAt = x.ClosedAt!.Value
            })
            .ToListAsync(cancellationToken);

        var totalPlatformCommissionYear = completedConsultationsCurrentYear
            .Sum(x => x.Price * platformCommissionRate);

        var monthlyPlatformCommission = BuildMonthlyRevenuePoints(
            completedConsultationsCurrentYear,
            now.Year,
            selector: x => x.ClosedAt,
            amountSelector: x => x.Price * platformCommissionRate);

        var dailyPlatformCommission = BuildDailyRevenuePoints(
            completedConsultationsCurrentYear,
            sevenDaysAgo,
            selector: x => x.ClosedAt,
            amountSelector: x => x.Price * platformCommissionRate);

        var monthlyNewDoctorCounts = await BuildMonthlyEntityCountsAsync(
            _context.Ophthalmologists.AsNoTracking().Select(x => x.CreatedAt),
            now.Year,
            cancellationToken);

        var monthlyNewOrganisationCounts = await BuildMonthlyEntityCountsAsync(
            _context.Organisations.AsNoTracking().Select(x => x.CreatedAt),
            now.Year,
            cancellationToken);

        var monthlyNewPatientCounts = await BuildMonthlyEntityCountsAsync(
            _context.Patients.AsNoTracking().Select(x => x.CreatedAt),
            now.Year,
            cancellationToken);

        var pendingOphthalmologistVerifications = await _context.Ophthalmologists
            .AsNoTracking()
            .CountAsync(x => x.VerificationStatus == VerificationStatus.PendingVerification, cancellationToken);

        var pendingOrganisationOnboarding = await _context.OrganisationOnboardingRequests
            .AsNoTracking()
            .CountAsync(x => x.Status == OrganisationOnboardingStatus.Pending, cancellationToken);

        var pendingWithdrawalRequests = await GetPendingWithdrawalRequestsAsync(cancellationToken);

        var liveConsultationSessions = await _context.ConsultationSessions
            .AsNoTracking()
            .CountAsync(x => x.ChatStatus == ChatStatus.Open, cancellationToken);

        var apiHealthy = true;
        var databaseHealthy = await IsDatabaseHealthyAsync(cancellationToken);

        var doctorRevenueById = completedConsultationsCurrentYear
            .Where(x => x.OphthalmologistId.HasValue)
            .GroupBy(x => x.OphthalmologistId!.Value)
            .Select(group => new
            {
                OphthalmologistId = group.Key,
                Revenue = group.Sum(x => x.Price)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(5)
            .ToList();

        var topDoctorIds = doctorRevenueById.Select(x => x.OphthalmologistId).ToList();
        var topDoctorProfiles = await (
            from doctor in _context.Ophthalmologists.AsNoTracking()
            join user in _context.Users.AsNoTracking() on doctor.UserId equals user.Id
            where topDoctorIds.Contains(doctor.Id)
            select new
            {
                doctor.Id,
                user.FullName,
                doctor.RatingAverage,
                doctor.RatingCount
            })
            .ToListAsync(cancellationToken);

        var topDoctorProfileById = topDoctorProfiles.ToDictionary(x => x.Id);
        var topDoctorsByConsultationRevenue = doctorRevenueById
            .Select(item =>
            {
                var profile = topDoctorProfileById.GetValueOrDefault(item.OphthalmologistId);
                return new DashboardTopDoctorDto
                {
                    OphthalmologistId = item.OphthalmologistId,
                    Name = profile?.FullName ?? string.Empty,
                    Revenue = item.Revenue,
                    RatingAverage = profile?.RatingAverage ?? 0m,
                    RatingCount = profile?.RatingCount ?? 0
                };
            })
            .ToList();

        var topOrganisationsByRating = await _context.Organisations
            .AsNoTracking()
            .OrderByDescending(x => x.RatingAverage)
            .ThenByDescending(x => x.RatingCount)
            .Take(5)
            .Select(x => new DashboardTopOrganisationDto
            {
                OrganisationId = x.Id,
                Name = x.Name,
                RatingAverage = x.RatingAverage,
                RatingCount = x.RatingCount
            })
            .ToListAsync(cancellationToken);

        return new DashboardMetricsDto
        {
            Doctors = doctorGrowth,
            Organisations = organisationGrowth,
            Patients = patientGrowth,
            PaymentMethodBreakdown = paymentMethodBreakdown,
            MonthlyRevenue = monthlyRevenue,
            DailyRevenue = dailyRevenue,
            TotalDepositRevenueYear = totalDepositRevenueYear,
            TotalPlatformCommissionYear = Math.Round(totalPlatformCommissionYear, 2),
            MonthlyPlatformCommission = monthlyPlatformCommission,
            DailyPlatformCommission = dailyPlatformCommission,
            MonthlyNewDoctorCounts = monthlyNewDoctorCounts,
            MonthlyNewOrganisationCounts = monthlyNewOrganisationCounts,
            MonthlyNewPatientCounts = monthlyNewPatientCounts,
            PendingActions = new DashboardPendingActionsDto
            {
                PendingOphthalmologistVerifications = pendingOphthalmologistVerifications,
                PendingWithdrawalRequests = pendingWithdrawalRequests,
                PendingOrganisationOnboarding = pendingOrganisationOnboarding
            },
            SystemStatus = new DashboardSystemStatusDto
            {
                LiveConsultationSessions = liveConsultationSessions,
                ApiHealthy = apiHealthy,
                DatabaseHealthy = databaseHealthy
            },
            TopDoctorsByConsultationRevenue = topDoctorsByConsultationRevenue,
            TopOrganisationsByRating = topOrganisationsByRating
        };
    }

    private static async Task<DashboardUserGrowthMetricDto> BuildGrowthMetricAsync(
        IQueryable<DateTime> createdAtQuery,
        DateTime previousMonthStart,
        DateTime currentMonthStart,
        DateTime nextMonthStart,
        CancellationToken cancellationToken)
    {
        var total = await createdAtQuery.CountAsync(cancellationToken);
        var currentMonth = await createdAtQuery
            .CountAsync(x => x >= currentMonthStart && x < nextMonthStart, cancellationToken);
        var previousMonth = await createdAtQuery
            .CountAsync(x => x >= previousMonthStart && x < currentMonthStart, cancellationToken);

        var growthPercentage = previousMonth == 0
            ? (currentMonth > 0 ? 100m : 0m)
            : Math.Round(((decimal)(currentMonth - previousMonth) / previousMonth) * 100m, 2);

        return new DashboardUserGrowthMetricDto
        {
            Total = total,
            CurrentMonth = currentMonth,
            PreviousMonth = previousMonth,
            GrowthPercentage = growthPercentage
        };
    }

    private static async Task<List<int>> BuildMonthlyEntityCountsAsync(
        IQueryable<DateTime> createdAtQuery,
        int year,
        CancellationToken cancellationToken)
    {
        var grouped = await createdAtQuery
            .Where(x => x.Year == year)
            .GroupBy(x => x.Month)
            .Select(group => new
            {
                group.Key,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        var byMonth = grouped.ToDictionary(x => x.Key, x => x.Count);
        var result = new List<int>(capacity: 12);

        for (var month = 1; month <= 12; month++)
        {
            result.Add(byMonth.GetValueOrDefault(month, 0));
        }

        return result;
    }

    private static List<DashboardRevenuePointDto> BuildMonthlyRevenuePoints<T>(
        IEnumerable<T> source,
        int year,
        Func<T, DateTime> selector,
        Func<T, decimal> amountSelector)
    {
        var grouped = source
            .Where(x => selector(x).Year == year)
            .GroupBy(x => selector(x).Month)
            .ToDictionary(group => group.Key, group => group.Sum(amountSelector));

        var points = new List<DashboardRevenuePointDto>(capacity: 12);
        for (var month = 1; month <= 12; month++)
        {
            var monthDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            points.Add(new DashboardRevenuePointDto
            {
                Month = month,
                Date = monthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Label = monthDate.ToString("MMM", CultureInfo.InvariantCulture),
                Revenue = Math.Round(grouped.GetValueOrDefault(month, 0m), 2)
            });
        }

        return points;
    }

    private static List<DashboardRevenuePointDto> BuildDailyRevenuePoints<T>(
        IEnumerable<T> source,
        DateTime fromDateInclusive,
        Func<T, DateTime> selector,
        Func<T, decimal> amountSelector)
    {
        var toDateInclusive = DateTime.UtcNow.Date;
        var grouped = source
            .Where(x => selector(x).Date >= fromDateInclusive.Date && selector(x).Date <= toDateInclusive)
            .GroupBy(x => selector(x).Date)
            .ToDictionary(group => group.Key, group => group.Sum(amountSelector));

        var points = new List<DashboardRevenuePointDto>();
        var current = fromDateInclusive.Date;
        while (current <= toDateInclusive)
        {
            points.Add(new DashboardRevenuePointDto
            {
                Date = current.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Label = current.ToString("dd MMM", CultureInfo.InvariantCulture),
                Revenue = Math.Round(grouped.GetValueOrDefault(current, 0m), 2)
            });

            current = current.AddDays(1);
        }

        return points;
    }

    private async Task<decimal> GetPlatformCommissionRateAsync(CancellationToken cancellationToken)
    {
        var raw = await _context.SystemSettings
            .AsNoTracking()
            .Where(x => x.Key == "DEFAULT_PLATFORM_COMMISSION")
            .Select(x => x.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            && parsed >= 0m
            && parsed <= 1m)
        {
            return parsed;
        }

        return 0.2m;
    }

    private async Task<bool> IsDatabaseHealthyAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }

    private async Task<int> GetPendingWithdrawalRequestsAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Database
                .SqlQueryRaw<int>(
                    @"SELECT COUNT(*)::int AS ""Value""
                      FROM ""WithdrawalRequests""
                      WHERE ""Status"" IN ('Pending', 'Processing')")
                .SingleAsync(cancellationToken);
        }
        catch
        {
            return 0;
        }
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

        var items = pageRows.Select(item => new RecentScreeningDto
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

        var quota = await _aiQuotaService.GetQuotaAsync(userId, Roles.OrgAdmin, cancellationToken);

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

        var quota = await _aiQuotaService.GetQuotaAsync(userId, Roles.Patient, cancellationToken);

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