namespace Application.Common.Interfaces;

/// <summary>
/// PayOS Service for Vietnam domestic payments (QR, Bank Transfer, Wallet)
/// </summary>
public interface IPayOSService
{
    /// <summary>
    /// Create payment link for PayOS (VND currency only)
    /// </summary>
    /// <param name="depositRequestId">Internal deposit request ID</param>
    /// <param name="amountVnd">Amount in VND</param>
    /// <param name="description">Payment description</param>
    /// <param name="returnUrl">URL to redirect after payment</param>
    /// <param name="cancelUrl">URL to redirect if payment cancelled</param>
    /// <returns>Payment URL and PayOS order code</returns>
    Task<(string PaymentUrl, string OrderCode)> CreatePaymentLinkAsync(
        Guid depositRequestId,
        decimal amountVnd,
        string description,
        string returnUrl,
        string cancelUrl);

    /// <summary>
    /// Query payment status from PayOS
    /// </summary>
    /// <param name="orderCode">PayOS order code</param>
    /// <returns>Status, Amount, and Transaction Reference</returns>
    Task<(string Status, decimal Amount, string TxnRef)> GetPaymentStatusAsync(string orderCode);

    /// <summary>
    /// Verify PayOS webhook signature
    /// </summary>
    /// <param name="signature">Signature from webhook</param>
    /// <param name="payload">Webhook payload</param>
    /// <returns>True if valid</returns>
    Task<bool> VerifyWebhookSignatureAsync(string signature, string payload);

    /// <summary>
    /// Cancel a pending payment
    /// </summary>
    /// <param name="orderCode">PayOS order code</param>
    /// <returns>True if cancelled successfully</returns>
    Task<bool> CancelPaymentAsync(string orderCode);
}
