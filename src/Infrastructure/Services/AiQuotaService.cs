using Application.AiQuota.Common;
using Application.AiQuota.Interfaces;
using Application.SystemSettings.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// AI Quota service — Stored State architecture.
/// Reads PurchasedAiQuota / UsedAiQuota directly from Patient or Organisation.
/// No JOINs to AiScreenings or WalletTransactions.
/// </summary>
public class AiQuotaService : IAiQuotaService
{
    private const decimal DefaultUnitPrice = 10000m;

    private readonly ApplicationDbContext _context;
    private readonly ILogger<AiQuotaService> _logger;
    private readonly ISystemSettingService _settingService;

    public AiQuotaService(
        ApplicationDbContext context,
        ILogger<AiQuotaService> logger,
        ISystemSettingService settingService)
    {
        _context = context;
        _logger = logger;
        _settingService = settingService;
    }

    public async Task<AiQuotaDto> GetQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[AiQuotaService] GetQuotaAsync called — UserId: {UserId}, Role: {Role}", userId, role);

        if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase))
            return await GetPatientQuotaAsync(userId, cancellationToken);

        if (IsOrganisationQuotaRole(role))
            return await GetOrgQuotaAsync(userId, cancellationToken);

        _logger.LogWarning("[AiQuotaService] Unrecognized role '{Role}' for user {UserId} — returning None quota", role, userId);
        return new AiQuotaDto
        {
            TotalQuota = 0,
            UsedQuota = 0,
            RemainingQuota = 0,
            QuotaSource = "None"
        };
    }

    public async Task<bool> HasAvailableQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var quota = await GetQuotaAsync(userId, role, cancellationToken);
        return quota.RemainingQuota > 0;
    }

    private async Task<AiQuotaDto> GetPatientQuotaAsync(Guid userId, CancellationToken cancellationToken)
    {
        var freeQuota = await GetSettingIntAsync("FREE_AI_QUOTA", 3, cancellationToken);
        var configuredUnitPrice = await GetSettingDecimalAsync("AI_QUOTA_UNIT_PRICE", DefaultUnitPrice, cancellationToken);
        var unitPrice = NormalizeUnitPrice(configuredUnitPrice);

        // Also try with IgnoreQueryFilters to detect if record exists but is soft-deleted
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (patient is null)
        {
            // Check if soft-deleted record exists for diagnostics
            var softDeleted = await _context.Patients
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

            if (softDeleted is not null)
                _logger.LogWarning(
                    "[AiQuotaService] Patient profile for UserId {UserId} exists but IsDeleted=true — returning free quota fallback",
                    userId);
            else
                _logger.LogWarning(
                    "[AiQuotaService] No Patient profile found for UserId {UserId} — returning free quota fallback",
                    userId);

            // Fallback: even without a Patient record, give the user their free daily quota
            return new AiQuotaDto
            {
                TotalQuota = freeQuota,
                UsedQuota = 0,
                RemainingQuota = freeQuota,
                QuotaSource = "Free",
                BundlePrice = unitPrice,
                BundleSize = 1,
                UnitPrice = unitPrice
            };
        }

        _logger.LogDebug(
            "[AiQuotaService] Patient {PatientId} found — PurchasedAiQuota: {Purchased}, UsedAiQuota: {Used}, FreeQuota: {Free}",
            patient.Id, patient.PurchasedAiQuota, patient.UsedAiQuota, freeQuota);

        var remainingFreeQuota = Math.Max(0, freeQuota - patient.UsedAiQuota);
        var totalQuota = freeQuota + patient.PurchasedAiQuota;
        var remaining = remainingFreeQuota + patient.PurchasedAiQuota;
        var quotaSource = remainingFreeQuota > 0
            ? "Free"
            : patient.PurchasedAiQuota > 0
                ? "Purchased"
                : "None";

        return new AiQuotaDto
        {
            TotalQuota = totalQuota,
            UsedQuota = patient.UsedAiQuota,
            RemainingQuota = remaining,
            QuotaSource = quotaSource,
            BundlePrice = unitPrice,
            BundleSize = 1,
            UnitPrice = unitPrice
        };
    }

    private static decimal NormalizeUnitPrice(decimal unitPrice)
    {
        return unitPrice > 0m ? unitPrice : DefaultUnitPrice;
    }

    private async Task<AiQuotaDto> GetOrgQuotaAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Find the organisation this user belongs to
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user?.OrganizationId is null)
            return new AiQuotaDto { TotalQuota = 0, UsedQuota = 0, RemainingQuota = 0, QuotaSource = "None" };

        var org = await _context.Organisations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == user.OrganizationId.Value, cancellationToken);

        if (org is null)
            return new AiQuotaDto { TotalQuota = 0, UsedQuota = 0, RemainingQuota = 0, QuotaSource = "None" };

        var freeQuota = await GetSettingIntAsync("FREE_AI_QUOTA", 3, cancellationToken);
        var configuredUnitPrice = await GetSettingDecimalAsync("AI_QUOTA_UNIT_PRICE", DefaultUnitPrice, cancellationToken);
        var unitPrice = NormalizeUnitPrice(configuredUnitPrice);
        var remainingFreeQuota = Math.Max(0, freeQuota - org.UsedAiQuota);
        var totalQuota = freeQuota + org.PurchasedAiQuota;
        var remaining = remainingFreeQuota + org.PurchasedAiQuota;

        return new AiQuotaDto
        {
            TotalQuota = totalQuota,
            UsedQuota = org.UsedAiQuota,
            RemainingQuota = remaining,
            UnitPrice = unitPrice,
            QuotaSource = remainingFreeQuota > 0
                ? "Free"
                : org.PurchasedAiQuota > 0
                    ? "Purchased"
                    : "None"
        };
    }

    public async Task DeductQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var freeQuota = await GetSettingIntAsync("FREE_AI_QUOTA", 3, cancellationToken);

        if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase))
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken)
                ?? throw new InvalidOperationException("Patient not found.");

            patient.ConsumeQuota(freeQuota);
        }
        else if (IsOrganisationQuotaRole(role))
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user?.OrganizationId is null)
                throw new InvalidOperationException("Organisation not found for this user.");

            var org = await _context.Organisations
                .FirstOrDefaultAsync(o => o.Id == user.OrganizationId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Organisation not found.");

            org.ConsumeQuota(freeQuota);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPurchasedQuotaAsync(Guid userId, string role, int amount, CancellationToken cancellationToken = default)
    {
        if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase))
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken)
                ?? throw new InvalidOperationException("Patient not found.");

            patient.AddPurchasedQuota(amount);
        }
        else if (IsOrganisationQuotaRole(role))
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user?.OrganizationId is null)
                throw new InvalidOperationException("Organisation not found for this user.");

            var org = await _context.Organisations
                .FirstOrDefaultAsync(o => o.Id == user.OrganizationId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Organisation not found.");

            org.AddPurchasedQuota(amount);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> GetSettingIntAsync(string key, int defaultValue, CancellationToken cancellationToken)
    {
        var valueStr = await _settingService.GetSettingAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(valueStr) && int.TryParse(valueStr, out var value) ? value : defaultValue;
    }

    private async Task<decimal> GetSettingDecimalAsync(string key, decimal defaultValue, CancellationToken cancellationToken)
    {
        var valueStr = await _settingService.GetSettingAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(valueStr) && decimal.TryParse(valueStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var value) ? value : defaultValue;
    }

    private static bool IsOrganisationQuotaRole(string role)
    {
        return string.Equals(role, "Ophthalmologist", StringComparison.OrdinalIgnoreCase)
            || string.Equals(role, "OrgAdmin", StringComparison.OrdinalIgnoreCase);
    }
}
