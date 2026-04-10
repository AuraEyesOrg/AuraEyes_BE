namespace Infrastructure.Settings;

/// <summary>
/// PayOS configuration settings for payment processing.
/// Mapped from appsettings.json "PayOS" section.
///
/// NOTE: PayOS has TWO separate APIs with different credentials:
///   - Payment API (api.payos.vn)   → ClientId + ApiKey + ChecksumKey
///   - Payout API  (api-merchant.payos.vn) → PayoutClientId + PayoutApiKey + ChecksumKey (shared)
/// </summary>
public sealed class PayOSSettings
{
    public const string SectionName = "PayOS";

    // ── Payment API credentials (nạp tiền / QR) ──────────────────────────

    /// <summary>PayOS Client ID (Payment API).</summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>PayOS API Key (Payment API).</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>PayOS Checksum Key (dùng chung cho cả Payment và Payout).</summary>
    public string ChecksumKey { get; set; } = string.Empty;

    /// <summary>Default return URL after successful payment.</summary>
    public string DefaultReturnUrl { get; set; } = string.Empty;

    /// <summary>Default cancel URL when payment is cancelled.</summary>
    public string DefaultCancelUrl { get; set; } = string.Empty;

    // ── Payout API credentials (chi tiền / disbursement) ─────────────────

    /// <summary>
    /// PayOS Payout Client ID — lấy từ trang merchant PayOS mục "Chi tiền".
    /// Có thể giống ClientId nếu cùng tài khoản, nhưng thường là key riêng.
    /// </summary>
    public string PayoutClientId { get; set; } = string.Empty;

    /// <summary>
    /// PayOS Payout API Key — lấy từ trang merchant PayOS mục "Chi tiền".
    /// </summary>
    public string PayoutApiKey { get; set; } = string.Empty;
}
