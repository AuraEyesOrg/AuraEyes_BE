using Application.AiQuota.Interfaces;
using Application.Common.Constants;
using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using Application.Organisations.Queries.GetDashboardMetrics;
using Application.Patients.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkload;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkloads;
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
    private const string FullTimeRequiredHoursWeekSettingKey = "FULLTIME_REQUIRED_HOURS_WEEK";
    private const string FullTimeRequiredHoursMonthSettingKey = "FULLTIME_REQUIRED_HOURS_MONTH";

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

        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = currentMonthStart.AddMonths(1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);

        var doctorTotalCount = await _context.Ophthalmologists.AsNoTracking()
            .CountAsync(cancellationToken);
        var doctorCurrentMonthCount = await _context.Ophthalmologists.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= currentMonthStart && item.CreatedAt < nextMonthStart, cancellationToken);
        var doctorPreviousMonthCount = await _context.Ophthalmologists.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= previousMonthStart && item.CreatedAt < currentMonthStart, cancellationToken);

        var doctorStats = new
        {
            Total = doctorTotalCount,
            CurrentMonth = doctorCurrentMonthCount,
            PreviousMonth = doctorPreviousMonthCount
        };

        var organisationTotalCount = await _context.Organisations.AsNoTracking()
            .CountAsync(cancellationToken);
        var organisationCurrentMonthCount = await _context.Organisations.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= currentMonthStart && item.CreatedAt < nextMonthStart, cancellationToken);
        var organisationPreviousMonthCount = await _context.Organisations.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= previousMonthStart && item.CreatedAt < currentMonthStart, cancellationToken);

        var organisationStats = new
        {
            Total = organisationTotalCount,
            CurrentMonth = organisationCurrentMonthCount,
            PreviousMonth = organisationPreviousMonthCount
        };

        var patientTotalCount = await _context.Patients.AsNoTracking()
            .CountAsync(cancellationToken);
        var patientCurrentMonthCount = await _context.Patients.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= currentMonthStart && item.CreatedAt < nextMonthStart, cancellationToken);
        var patientPreviousMonthCount = await _context.Patients.AsNoTracking()
            .CountAsync(item => item.CreatedAt >= previousMonthStart && item.CreatedAt < currentMonthStart, cancellationToken);

        var patientStats = new
        {
            Total = patientTotalCount,
            CurrentMonth = patientCurrentMonthCount,
            PreviousMonth = patientPreviousMonthCount
        };
        var doctorTotal = doctorStats?.Total ?? 0;
        var doctorCurrentMonth = doctorStats?.CurrentMonth ?? 0;
        var doctorPreviousMonth = doctorStats?.PreviousMonth ?? 0;

        var organisationTotal = organisationStats?.Total ?? 0;
        var organisationCurrentMonth = organisationStats?.CurrentMonth ?? 0;
        var organisationPreviousMonth = organisationStats?.PreviousMonth ?? 0;

        var patientTotal = patientStats?.Total ?? 0;
        var patientCurrentMonth = patientStats?.CurrentMonth ?? 0;
        var patientPreviousMonth = patientStats?.PreviousMonth ?? 0;

        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextYearStart = yearStart.AddYears(1);

        var paymentMethodRevenueRaw = await _context.DepositRequests.AsNoTracking()
            .Where(deposit =>
                deposit.Status == PaymentStatus.Completed &&
                (deposit.CompletedAt ?? deposit.CreatedAt) >= yearStart &&
                (deposit.CompletedAt ?? deposit.CreatedAt) < nextYearStart)
            .GroupBy(deposit => deposit.PaymentMethod)
            .Select(group => new
            {
                PaymentMethod = group.Key,
                Amount = group.Sum(item => item.Amount)
            })
            .ToListAsync(cancellationToken);

        var monthlyRevenueRaw = await _context.DepositRequests.AsNoTracking()
            .Where(deposit =>
                deposit.Status == PaymentStatus.Completed &&
                (deposit.CompletedAt ?? deposit.CreatedAt) >= yearStart &&
                (deposit.CompletedAt ?? deposit.CreatedAt) < nextYearStart)
            .GroupBy(deposit => (deposit.CompletedAt ?? deposit.CreatedAt).Month)
            .Select(group => new
            {
                Month = group.Key,
                Revenue = group.Sum(item => item.Amount)
            })
            .ToListAsync(cancellationToken);

        var sevenDaysStart = now.Date.AddDays(-6);
        var nextDay = now.Date.AddDays(1);
        var dailyRevenueRaw = await _context.DepositRequests.AsNoTracking()
            .Where(deposit =>
                deposit.Status == PaymentStatus.Completed &&
                (deposit.CompletedAt ?? deposit.CreatedAt) >= sevenDaysStart &&
                (deposit.CompletedAt ?? deposit.CreatedAt) < nextDay)
            .GroupBy(deposit => (deposit.CompletedAt ?? deposit.CreatedAt).Date)
            .Select(group => new
            {
                Date = group.Key,
                Revenue = group.Sum(item => item.Amount)
            })
            .ToListAsync(cancellationToken);
        var totalDepositAmount = paymentMethodRevenueRaw.Sum(item => item.Amount);

        var paymentMethodBreakdown = paymentMethodRevenueRaw
            .Select(item =>
            {
                var amount = item.Amount;
                return new PaymentMethodRevenueDto
                {
                    PaymentMethod = item.PaymentMethod.ToString(),
                    Amount = amount,
                    Percentage = totalDepositAmount <= 0m ? 0m : Math.Round(amount / totalDepositAmount * 100m, 1)
                };
            })
            .OrderByDescending(item => item.Amount)
            .ToList();

        var monthlyRevenueMap = monthlyRevenueRaw
            .ToDictionary(item => item.Month, item => Math.Round(item.Revenue, 0));
        var monthlyRevenue = Enumerable.Range(1, 12)
            .Select(month => new MonthlyRevenuePointDto
            {
                Month = month,
                Label = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(month),
                Revenue = monthlyRevenueMap.GetValueOrDefault(month, 0m)
            })
            .ToList();

        var dailyRevenueMap = dailyRevenueRaw
            .ToDictionary(item => item.Date, item => Math.Round(item.Revenue, 0));
        var dailyRevenue = Enumerable.Range(0, 7)
            .Select(offset => sevenDaysStart.AddDays(offset))
            .Select(date => new DailyRevenuePointDto
            {
                Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                Label = date.ToString("dd MMM", CultureInfo.InvariantCulture),
                Revenue = dailyRevenueMap.GetValueOrDefault(date, 0m)
            })
            .ToList();

        var monthlyPlatformRaw = await (
            from t in _context.WalletTransactions.AsNoTracking()
            join w in _context.Wallets.AsNoTracking() on t.WalletId equals w.Id
            where w.OwnerType == "System"
                  && t.TransactionType == TransactionType.Deposit
                  && t.ReferenceType == "Booking"
                  && t.CreatedAt >= yearStart
                  && t.CreatedAt < nextYearStart
            group t by t.CreatedAt.Month
            into g
            select new
            {
                Month = g.Key,
                Revenue = g.Sum(x => x.Amount)
            }).ToListAsync(cancellationToken);

        var dailyPlatformRaw = await (
            from t in _context.WalletTransactions.AsNoTracking()
            join w in _context.Wallets.AsNoTracking() on t.WalletId equals w.Id
            where w.OwnerType == "System"
                  && t.TransactionType == TransactionType.Deposit
                  && t.ReferenceType == "Booking"
                  && t.CreatedAt >= sevenDaysStart
                  && t.CreatedAt < nextDay
            group t by t.CreatedAt.Date
            into g
            select new
            {
                Date = g.Key,
                Revenue = g.Sum(x => x.Amount)
            }).ToListAsync(cancellationToken);

        var monthlyPlatformMap = monthlyPlatformRaw
            .ToDictionary(item => item.Month, item => Math.Round(item.Revenue, 0));
        var monthlyPlatformCommission = Enumerable.Range(1, 12)
            .Select(month => new MonthlyRevenuePointDto
            {
                Month = month,
                Label = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(month),
                Revenue = monthlyPlatformMap.GetValueOrDefault(month, 0m)
            })
            .ToList();

        var dailyPlatformMap = dailyPlatformRaw
            .ToDictionary(item => item.Date, item => Math.Round(item.Revenue, 0));
        var dailyPlatformCommission = Enumerable.Range(0, 7)
            .Select(offset => sevenDaysStart.AddDays(offset))
            .Select(date => new DailyRevenuePointDto
            {
                Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                Label = date.ToString("dd MMM", CultureInfo.InvariantCulture),
                Revenue = dailyPlatformMap.GetValueOrDefault(date, 0m)
            })
            .ToList();

        var totalDepositRevenueYear = monthlyRevenue.Sum(m => m.Revenue);
        var totalPlatformCommissionYear = monthlyPlatformCommission.Sum(m => m.Revenue);

        var newDoctorsByMonth = await _context.Ophthalmologists.AsNoTracking()
            .Where(o => o.CreatedAt >= yearStart && o.CreatedAt < nextYearStart)
            .GroupBy(o => o.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        var newOrgsByMonth = await _context.Organisations.AsNoTracking()
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
        var orgMonthMap = newOrgsByMonth.ToDictionary(x => x.Month, x => x.Count);
        var patientMonthMap = newPatientsByMonth.ToDictionary(x => x.Month, x => x.Count);
        var monthlyNewDoctorCounts = Enumerable.Range(1, 12).Select(m => doctorMonthMap.GetValueOrDefault(m, 0)).ToList();
        var monthlyNewOrganisationCounts = Enumerable.Range(1, 12).Select(m => orgMonthMap.GetValueOrDefault(m, 0)).ToList();
        var monthlyNewPatientCounts = Enumerable.Range(1, 12).Select(m => patientMonthMap.GetValueOrDefault(m, 0)).ToList();

        var pendingDoctorVerifications = await _context.Ophthalmologists
            .CountAsync(o => o.VerificationStatus == VerificationStatus.PendingVerification, cancellationToken);
        var pendingWithdrawals = await _context.WithdrawalRequests
            .CountAsync(
                w => w.Status == PaymentStatus.Pending || w.Status == PaymentStatus.Processing,
                cancellationToken);
        var pendingOnboarding = await _context.OrganisationOnboardingRequests
            .CountAsync(r => r.Status == OrganisationOnboardingStatus.Pending, cancellationToken);

        var liveConsultations = await _context.ConsultationSessions
            .CountAsync(
                s => s.ChatStatus == ChatStatus.Open
                     && s.Status != SessionStatus.Completed
                     && s.Status != SessionStatus.Cancelled,
                cancellationToken);

        var databaseHealthy = await _context.Database.CanConnectAsync(cancellationToken);

        // Consultation credits: Deposit or Transfer on ophthalmologist wallets. ReferenceType is usually
        var topDoctorRows = await (
            from t in _context.WalletTransactions.AsNoTracking()
            join w in _context.Wallets.AsNoTracking() on t.WalletId equals w.Id
            join o in _context.Ophthalmologists.AsNoTracking() on w.UserId equals o.UserId
            join u in _context.Users.IgnoreQueryFilters().AsNoTracking() on o.UserId equals u.Id
            where w.OwnerType == "Ophthalmologist"
                  && (t.TransactionType == TransactionType.Deposit
                      || t.TransactionType == TransactionType.Transfer)
                  && (t.ReferenceType == "Booking")
            group t.Amount by new { o.Id, o.RatingAverage, o.RatingCount, FullName = u.FullName } into g
            select new TopPerformerDoctorDto
            {
                OphthalmologistId = g.Key.Id,
                Name = g.Key.FullName ?? string.Empty,
                Revenue = g.Sum(),
                RatingAverage = g.Key.RatingAverage,
                RatingCount = g.Key.RatingCount
            }).OrderByDescending(x => x.Revenue).Take(5).ToListAsync(cancellationToken);

        var topOrgRows = await _context.Organisations.AsNoTracking()
            .OrderByDescending(o => o.RatingAverage)
            .ThenByDescending(o => o.RatingCount)
            .Take(5)
            .Select(o => new TopPerformerOrganisationDto
            {
                OrganisationId = o.Id,
                Name = o.Name,
                RatingAverage = o.RatingAverage,
                RatingCount = o.RatingCount
            })
            .ToListAsync(cancellationToken);

        var monitorDescriptors = _betterStackHeartbeatService.GetMonitorDescriptors();

        return new DashboardMetricsDto
        {
            Doctors = new UserGrowthMetricDto
            {
                Total = doctorTotal,
                CurrentMonth = doctorCurrentMonth,
                PreviousMonth = doctorPreviousMonth,
                GrowthPercentage = CalculateGrowthPercentage(doctorCurrentMonth, doctorPreviousMonth)
            },
            Organisations = new UserGrowthMetricDto
            {
                Total = organisationTotal,
                CurrentMonth = organisationCurrentMonth,
                PreviousMonth = organisationPreviousMonth,
                GrowthPercentage = CalculateGrowthPercentage(organisationCurrentMonth, organisationPreviousMonth)
            },
            Patients = new UserGrowthMetricDto
            {
                Total = patientTotal,
                CurrentMonth = patientCurrentMonth,
                PreviousMonth = patientPreviousMonth,
                GrowthPercentage = CalculateGrowthPercentage(patientCurrentMonth, patientPreviousMonth)
            },
            PaymentMethodBreakdown = paymentMethodBreakdown,
            MonthlyRevenue = monthlyRevenue,
            DailyRevenue = dailyRevenue,
            TotalDepositRevenueYear = totalDepositRevenueYear,
            TotalPlatformCommissionYear = totalPlatformCommissionYear,
            MonthlyPlatformCommission = monthlyPlatformCommission,
            DailyPlatformCommission = dailyPlatformCommission,
            MonthlyNewDoctorCounts = monthlyNewDoctorCounts,
            MonthlyNewOrganisationCounts = monthlyNewOrganisationCounts,
            MonthlyNewPatientCounts = monthlyNewPatientCounts,
            PendingActions = new DashboardPendingActionsDto
            {
                PendingOphthalmologistVerifications = pendingDoctorVerifications,
                PendingWithdrawalRequests = pendingWithdrawals,
                PendingOrganisationOnboarding = pendingOnboarding
            },
            SystemStatus = new DashboardSystemStatusDto
            {
                LiveConsultationSessions = liveConsultations,
                ApiHealthy = true,
                DatabaseHealthy = databaseHealthy
            },
            BetterStack = new DashboardBetterStackDto
            {
                Enabled = monitorDescriptors.Any(item => item.Configured),
                EmbedUrl = _betterStackHeartbeatService.GetEmbedUrl(),
                Monitors = monitorDescriptors
                    .Select(item => new DashboardBackgroundMonitorDto
                    {
                        Key = item.Key,
                        Name = item.DisplayName,
                        Category = item.Category,
                        Configured = item.Configured
                    })
                    .ToList()
            },
            TopDoctorsByConsultationRevenue = topDoctorRows,
            TopOrganisationsByRating = topOrgRows
        };
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
            var normalizedSearchTerm = $"%{searchTerm.Trim()}%";
            doctorsQuery = doctorsQuery.Where(x =>
                EF.Functions.ILike(x.Name, normalizedSearchTerm)
                || (x.Email != null && EF.Functions.ILike(x.Email, normalizedSearchTerm)));
        }

        if (employmentType.HasValue)
        {
            doctorsQuery = doctorsQuery.Where(x => x.EmploymentType == employmentType.Value);
        }

        var doctors = await doctorsQuery.ToListAsync(cancellationToken);
        if (doctors.Count == 0)
        {
            return new PagedResult<DoctorWorkloadListItemDto>(
                new List<DoctorWorkloadListItemDto>(),
                0,
                pageNumber,
                pageSize);
        }

        var intervals = await QueryCompletedSessionIntervalsAsync(
            doctors.Select(x => x.Id).ToList(),
            periodBounds.StartUtc,
            periodBounds.EndUtcExclusive,
            cancellationToken);

        var intervalsByDoctor = intervals
            .GroupBy(x => x.DoctorId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var requiredHoursByEmploymentType = await GetRequiredHoursByEmploymentTypeAsync(periodType, cancellationToken);

        var computed = new List<DoctorWorkloadListItemDto>(doctors.Count);
        foreach (var doctor in doctors)
        {
            var doctorIntervals = intervalsByDoctor.TryGetValue(doctor.Id, out var value)
                ? value
                : new List<SessionIntervalProjection>();

            var actualHours = CalculateMergedHours(doctorIntervals, periodBounds.StartUtc, periodBounds.EndUtcExclusive);
            var requiredHours = requiredHoursByEmploymentType.TryGetValue(doctor.EmploymentType, out var required)
                ? required
                : 0m;

            var completionRate = CalculateCompletionRate(actualHours, requiredHours);
            var workloadStatus = actualHours >= requiredHours ? "OK" : "UNDER";
            var warningFlag = completionRate < 0.8m;

            if (normalizedStatus is not null && !string.Equals(workloadStatus, normalizedStatus, StringComparison.Ordinal))
                continue;

            if (warningOnly && !warningFlag)
                continue;

            computed.Add(new DoctorWorkloadListItemDto
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
            });
        }

        var ordered = computed
            .OrderByDescending(item => item.WarningFlag)
            .ThenBy(item => item.CompletionRate)
            .ThenBy(item => item.DoctorName)
            .ToList();

        var totalCount = ordered.Count;
        var pageItems = ordered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<DoctorWorkloadListItemDto>(pageItems, totalCount, pageNumber, pageSize);
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

    private async Task<decimal> GetRequiredHoursAsync(
        OphthalmologistEmploymentType employmentType,
        WorkloadPeriodType periodType,
        CancellationToken cancellationToken)
    {
        var requiredHours = await _context.WorkloadRequirements
            .AsNoTracking()
            .Where(x => x.EmploymentType == employmentType && x.PeriodType == periodType)
            .Select(x => (decimal?)x.RequiredHours)
            .FirstOrDefaultAsync(cancellationToken);

        if (employmentType == OphthalmologistEmploymentType.FullTime)
        {
            var configuredRequiredHours = await GetConfiguredFullTimeRequiredHoursAsync(periodType, cancellationToken);
            if (configuredRequiredHours.HasValue)
                return configuredRequiredHours.Value;
        }

        return requiredHours.GetValueOrDefault(0m);
    }

    private async Task<Dictionary<OphthalmologistEmploymentType, decimal>> GetRequiredHoursByEmploymentTypeAsync(
        WorkloadPeriodType periodType,
        CancellationToken cancellationToken)
    {
        var requiredHoursByEmploymentType = await _context.WorkloadRequirements
            .AsNoTracking()
            .Where(x => x.PeriodType == periodType)
            .GroupBy(x => x.EmploymentType)
            .Select(group => new
            {
                EmploymentType = group.Key,
                RequiredHours = group
                    .OrderByDescending(item => item.UpdatedAt ?? item.CreatedAt)
                    .Select(item => item.RequiredHours)
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.EmploymentType, x => x.RequiredHours, cancellationToken);

        var configuredRequiredHours = await GetConfiguredFullTimeRequiredHoursAsync(periodType, cancellationToken);
        if (configuredRequiredHours.HasValue)
        {
            requiredHoursByEmploymentType[OphthalmologistEmploymentType.FullTime] = configuredRequiredHours.Value;
        }

        return requiredHoursByEmploymentType;
    }

    private async Task<decimal?> GetConfiguredFullTimeRequiredHoursAsync(
        WorkloadPeriodType periodType,
        CancellationToken cancellationToken)
    {
        var settingKey = periodType == WorkloadPeriodType.Week
            ? FullTimeRequiredHoursWeekSettingKey
            : FullTimeRequiredHoursMonthSettingKey;

        var configuredValue = await _context.SystemSettings
            .AsNoTracking()
            .Where(x => x.Key == settingKey)
            .Select(x => x.Value)
            .FirstOrDefaultAsync(cancellationToken);

        return TryParseConfiguredRequiredHours(configuredValue);
    }

    private static decimal? TryParseConfiguredRequiredHours(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
            return null;

        if (parsed < 0m || parsed > 744m)
            return null;

        return Math.Round(parsed, 2, MidpointRounding.AwayFromZero);
    }

    private async Task<List<SessionIntervalProjection>> QueryCompletedSessionIntervalsAsync(
        IReadOnlyCollection<Guid> doctorIds,
        DateTime periodStartUtc,
        DateTime periodEndUtcExclusive,
        CancellationToken cancellationToken)
    {
        if (doctorIds.Count == 0)
            return new List<SessionIntervalProjection>();

        return await _context.ConsultationSessions
            .AsNoTracking()
            .Where(session =>
                session.OphthalmologistId.HasValue
                && doctorIds.Contains(session.OphthalmologistId.Value)
                && session.Status == SessionStatus.Completed
                && session.StartTime.HasValue
                && session.EndTime.HasValue
                && session.StartTime.Value < periodEndUtcExclusive
                && session.EndTime.Value > periodStartUtc)
            .Select(session => new SessionIntervalProjection
            {
                DoctorId = session.OphthalmologistId!.Value,
                StartUtc = session.StartTime!.Value,
                EndUtc = session.EndTime!.Value
            })
            .ToListAsync(cancellationToken);
    }

    private static decimal CalculateMergedHours(
        IEnumerable<SessionIntervalProjection> intervals,
        DateTime periodStartUtc,
        DateTime periodEndUtcExclusive)
    {
        var normalized = intervals
            .Select(interval =>
            {
                var clampedStart = interval.StartUtc < periodStartUtc ? periodStartUtc : interval.StartUtc;
                var clampedEnd = interval.EndUtc > periodEndUtcExclusive ? periodEndUtcExclusive : interval.EndUtc;

                return new WorkloadInterval
                {
                    StartUtc = clampedStart,
                    EndUtc = clampedEnd
                };
            })
            .Where(interval => interval.EndUtc > interval.StartUtc)
            .OrderBy(interval => interval.StartUtc)
            .ToList();

        if (normalized.Count == 0)
            return 0m;

        var mergedStart = normalized[0].StartUtc;
        var mergedEnd = normalized[0].EndUtc;
        decimal totalHours = 0m;

        foreach (var interval in normalized.Skip(1))
        {
            if (interval.StartUtc <= mergedEnd)
            {
                if (interval.EndUtc > mergedEnd)
                    mergedEnd = interval.EndUtc;

                continue;
            }

            totalHours += ConvertTicksToHours(mergedEnd - mergedStart);
            mergedStart = interval.StartUtc;
            mergedEnd = interval.EndUtc;
        }

        totalHours += ConvertTicksToHours(mergedEnd - mergedStart);
        return Math.Round(totalHours, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal ConvertTicksToHours(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            return 0m;

        return duration.Ticks / (decimal)TimeSpan.TicksPerHour;
    }

    private static DateOnly ResolveAnchorDate(DateOnly date)
    {
        if (date != default)
            return date;

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZoneResolver.TimeZone);
        return DateOnly.FromDateTime(localNow);
    }

    private static PeriodBounds GetPeriodBounds(WorkloadPeriodType periodType, DateOnly date)
    {
        var localDateTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);

        DateTime localStart;
        DateTime localEndExclusive;

        if (periodType == WorkloadPeriodType.Week)
        {
            var offset = ((int)localDateTime.DayOfWeek + 6) % 7;
            localStart = localDateTime.Date.AddDays(-offset);
            localEndExclusive = localStart.AddDays(7);
        }
        else
        {
            localStart = new DateTime(localDateTime.Year, localDateTime.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);
            localEndExclusive = localStart.AddMonths(1);
        }

        var periodStartUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, VietnamTimeZoneResolver.TimeZone);
        var periodEndUtcExclusive = TimeZoneInfo.ConvertTimeToUtc(localEndExclusive, VietnamTimeZoneResolver.TimeZone);

        return new PeriodBounds(
            periodStartUtc,
            periodEndUtcExclusive,
            periodEndUtcExclusive.AddTicks(-1));
    }

    private static decimal CalculateCompletionRate(decimal actualHours, decimal requiredHours)
    {
        if (requiredHours <= 0)
            return 1m;

        var completionRate = actualHours / requiredHours;
        if (completionRate < 0)
            return 0m;

        return Math.Round(completionRate, 2, MidpointRounding.AwayFromZero);
    }

    private static string ToApiPeriodType(WorkloadPeriodType periodType)
        => periodType == WorkloadPeriodType.Week ? "WEEK" : "MONTH";

    private static string ToApiEmploymentType(OphthalmologistEmploymentType employmentType)
        => employmentType == OphthalmologistEmploymentType.FullTime ? "FULL_TIME" : "PART_TIME";

    private static string? NormalizeStatusFilter(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        var normalized = status.Trim().ToUpperInvariant();
        return normalized is "OK" or "UNDER" ? normalized : null;
    }

    private sealed class DoctorProjection
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Email { get; init; }
        public OphthalmologistEmploymentType EmploymentType { get; init; }
    }

    private sealed class SessionIntervalProjection
    {
        public Guid DoctorId { get; init; }
        public DateTime StartUtc { get; init; }
        public DateTime EndUtc { get; init; }
    }

    private sealed class WorkloadInterval
    {
        public DateTime StartUtc { get; init; }
        public DateTime EndUtc { get; init; }
    }

    private readonly record struct PeriodBounds(
        DateTime StartUtc,
        DateTime EndUtcExclusive,
        DateTime EndUtcInclusive);
}