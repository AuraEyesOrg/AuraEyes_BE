using Application.AiQuota.Common;
using Application.AiQuota.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// AI Quota service — Stored State architecture.
/// Reads PurchasedAiQuota / UsedAiQuota directly from Patient or Organisation.
/// No JOINs to AiScreenings or WalletTransactions.
/// </summary>
public class AiQuotaService : IAiQuotaService
{
    private readonly ApplicationDbContext _context;

    public AiQuotaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AiQuotaDto> GetQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase))
            return await GetPatientQuotaAsync(userId, cancellationToken);

        if (string.Equals(role, "Ophthalmologist", StringComparison.OrdinalIgnoreCase))
            return await GetOrgQuotaAsync(userId, cancellationToken);

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
        var bundleSize = await GetSettingIntAsync("AI_QUOTA_BUNDLE", 5, cancellationToken);
        var bundlePrice = await GetSettingDecimalAsync("AI_QUOTA_PRICE", 50000m, cancellationToken);

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (patient is null)
            return new AiQuotaDto { TotalQuota = 0, UsedQuota = 0, RemainingQuota = 0, QuotaSource = "None" };

        var totalQuota = freeQuota + patient.PurchasedAiQuota;
        var remaining = Math.Max(0, totalQuota - patient.UsedAiQuota);
        var quotaSource = patient.PurchasedAiQuota > 0 ? "Purchased" : "Free";

        return new AiQuotaDto
        {
            TotalQuota = totalQuota,
            UsedQuota = patient.UsedAiQuota,
            RemainingQuota = remaining,
            QuotaSource = quotaSource,
            BundlePrice = bundlePrice,
            BundleSize = bundleSize
        };
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
        var totalQuota = freeQuota + org.PurchasedAiQuota;
        var remaining = Math.Max(0, totalQuota - org.UsedAiQuota);

        return new AiQuotaDto
        {
            TotalQuota = totalQuota,
            UsedQuota = org.UsedAiQuota,
            RemainingQuota = remaining,
            QuotaSource = "Organisation"
        };
    }

    public async Task DeductQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase))
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken)
                ?? throw new InvalidOperationException("Patient not found.");

            patient.IncrementUsedQuota();
        }
        else if (string.Equals(role, "Ophthalmologist", StringComparison.OrdinalIgnoreCase))
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user?.OrganizationId is null)
                throw new InvalidOperationException("Organisation not found for this user.");

            var org = await _context.Organisations
                .FirstOrDefaultAsync(o => o.Id == user.OrganizationId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Organisation not found.");

            org.IncrementUsedQuota();
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
        else if (string.Equals(role, "Ophthalmologist", StringComparison.OrdinalIgnoreCase))
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
        var setting = await _context.Set<Domain.Entities.Platform.SystemSetting>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        return setting is not null && int.TryParse(setting.Value, out var value) ? value : defaultValue;
    }

    private async Task<decimal> GetSettingDecimalAsync(string key, decimal defaultValue, CancellationToken cancellationToken)
    {
        var setting = await _context.Set<Domain.Entities.Platform.SystemSetting>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        return setting is not null && decimal.TryParse(setting.Value, out var value) ? value : defaultValue;
    }
}
