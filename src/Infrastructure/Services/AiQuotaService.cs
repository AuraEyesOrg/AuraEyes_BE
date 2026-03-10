using Application.AiQuota.Common;
using Application.AiQuota.Interfaces;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// AI Quota service implementation using ApplicationDbContext.
/// Calculates quota based on user role:
/// - Patient (B2C): FREE_AI_QUOTA + purchased bundles
/// - Organisation (B2B): sum of active Contract.AiQuotaLimit
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
            return await GetContractQuotaAsync(userId, cancellationToken);

        // Default fallback for other roles
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
        // Get system settings
        var freeQuota = await GetSettingIntAsync("FREE_AI_QUOTA", 3, cancellationToken);
        var bundleSize = await GetSettingIntAsync("AI_QUOTA_BUNDLE", 5, cancellationToken);
        var bundlePrice = await GetSettingDecimalAsync("AI_QUOTA_PRICE", 50000m, cancellationToken);

        // Get patient profile
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (patient is null)
            return new AiQuotaDto { TotalQuota = 0, UsedQuota = 0, RemainingQuota = 0, QuotaSource = "None" };

        // Count purchased bundles via wallet transactions
        var wallet = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

        var purchasedBundles = 0;
        if (wallet is not null)
        {
            purchasedBundles = await _context.WalletTransactions
                .AsNoTracking()
                .CountAsync(t => t.WalletId == wallet.Id
                    && t.ReferenceType == "AiQuota"
                    && t.TransactionType == TransactionType.Payment,
                    cancellationToken);
        }

        // Count used screenings
        var usedQuota = await _context.AiScreenings
            .AsNoTracking()
            .CountAsync(s => s.PatientId == patient.Id && s.IsActive, cancellationToken);

        var totalQuota = freeQuota + (purchasedBundles * bundleSize);
        var quotaSource = purchasedBundles > 0 ? "Purchased" : "Free";

        return new AiQuotaDto
        {
            TotalQuota = totalQuota,
            UsedQuota = usedQuota,
            RemainingQuota = Math.Max(0, totalQuota - usedQuota),
            QuotaSource = quotaSource,
            BundlePrice = bundlePrice,
            BundleSize = bundleSize
        };
    }

    private async Task<AiQuotaDto> GetContractQuotaAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Sum AiQuotaLimit from all active contracts for this user
        var totalQuota = await _context.Contracts
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.Status == ContractStatus.Active)
            .SumAsync(c => c.AiQuotaLimit, cancellationToken);

        // Count screenings created by patients under this ophthalmologist's organisation
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var usedQuota = 0;
        if (user?.OrganizationId is not null)
        {
            // Get all patients in this organisation
            var orgPatientIds = await _context.Users
                .AsNoTracking()
                .Where(u => u.OrganizationId == user.OrganizationId && !u.IsDeleted)
                .Join(_context.Patients,
                    u => u.Id,
                    p => p.UserId,
                    (u, p) => p.Id)
                .ToListAsync(cancellationToken);

            usedQuota = await _context.AiScreenings
                .AsNoTracking()
                .CountAsync(s => orgPatientIds.Contains(s.PatientId) && s.IsActive, cancellationToken);
        }

        return new AiQuotaDto
        {
            TotalQuota = totalQuota,
            UsedQuota = usedQuota,
            RemainingQuota = Math.Max(0, totalQuota - usedQuota),
            QuotaSource = "Contract"
        };
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
