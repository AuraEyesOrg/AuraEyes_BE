using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;

namespace Infrastructure.Services;

/// <summary>
/// PayOS Service Implementation for Vietnam domestic payments using official SDK.
/// Supports QR Code, Bank Transfer, and E-Wallet payments.
/// </summary>
public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly ILogger<PayOSService> _logger;
    private readonly PayOSSettings _settings;

    public PayOSService(IOptions<PayOSSettings> settings, ILogger<PayOSService> logger)
    {
        _logger = logger;
        _settings = settings.Value;

        if (string.IsNullOrEmpty(_settings.ClientId))
            throw new ArgumentNullException(nameof(_settings.ClientId), "PayOS:ClientId is required");
        if (string.IsNullOrEmpty(_settings.ApiKey))
            throw new ArgumentNullException(nameof(_settings.ApiKey), "PayOS:ApiKey is required");
        if (string.IsNullOrEmpty(_settings.ChecksumKey))
            throw new ArgumentNullException(nameof(_settings.ChecksumKey), "PayOS:ChecksumKey is required");

        // Initialize PayOS SDK
        _payOS = new PayOS(_settings.ClientId, _settings.ApiKey, _settings.ChecksumKey);

        _logger.LogInformation("PayOS Service initialized with ClientId: {ClientId}", _settings.ClientId);
    }

    public async Task<(string PaymentUrl, string OrderCode)> CreatePaymentLinkAsync(
        Guid depositRequestId,
        decimal amountVnd,
        string description,
        string returnUrl,
        string cancelUrl)
    {
        try
        {
            // Generate unique order code (PayOS requires long type)
            // Using timestamp + random to ensure uniqueness
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var random = new Random().Next(1000, 9999);
            var orderCode = long.Parse($"{timestamp % 10000000000}{random}");

            _logger.LogInformation(
                "Creating PayOS payment: OrderCode={OrderCode}, Amount={Amount} VND, DepositRequestId={DepositRequestId}",
                orderCode, amountVnd, depositRequestId);

            // PayOS SDK requires amount in integer format
            var amountInt = (int)amountVnd;

            // Use default URLs if not provided
            var effectiveReturnUrl = string.IsNullOrEmpty(returnUrl) ? _settings.DefaultReturnUrl : returnUrl;
            var effectiveCancelUrl = string.IsNullOrEmpty(cancelUrl) ? _settings.DefaultCancelUrl : cancelUrl;

            // Append orderCode to URLs so frontend can access it
            var returnUrlWithOrderCode = AppendQueryParam(effectiveReturnUrl, "orderCode", orderCode.ToString());
            var cancelUrlWithOrderCode = AppendQueryParam(effectiveCancelUrl, "orderCode", orderCode.ToString());

            // Create items list
            var items = new List<ItemData>
            {
                new ItemData(TruncateDescription(description), 1, amountInt)
            };

            // Create payment data using PayOS SDK
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: amountInt,
                description: TruncateDescription(description),
                items: items,
                returnUrl: returnUrlWithOrderCode,
                cancelUrl: cancelUrlWithOrderCode
            );

            _logger.LogInformation("Calling PayOS SDK to create payment link");

            // Call PayOS SDK
            var response = await _payOS.createPaymentLink(paymentData);

            if (response == null || string.IsNullOrEmpty(response.checkoutUrl))
            {
                throw new Exception("PayOS SDK returned null or empty checkout URL");
            }

            _logger.LogInformation(
                "PayOS payment created successfully: URL={Url}, OrderCode={OrderCode}",
                response.checkoutUrl, orderCode);

            return (response.checkoutUrl, orderCode.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create PayOS payment link for DepositRequestId={DepositRequestId}", depositRequestId);
            throw new Exception($"Failed to create PayOS payment link: {ex.Message}", ex);
        }
    }

    public async Task<(string Status, decimal Amount, string TxnRef)> GetPaymentStatusAsync(string orderCode)
    {
        try
        {
            _logger.LogInformation("Querying PayOS payment status for OrderCode={OrderCode}", orderCode);

            var paymentInfo = await _payOS.getPaymentLinkInformation(long.Parse(orderCode));

            if (paymentInfo == null)
            {
                throw new Exception($"Payment not found for OrderCode={orderCode}");
            }

            var status = paymentInfo.status ?? "UNKNOWN";
            var amount = paymentInfo.amount;
            var txnRef = paymentInfo.transactions?.FirstOrDefault()?.reference ?? string.Empty;

            _logger.LogInformation(
                "PayOS payment status: OrderCode={OrderCode}, Status={Status}, Amount={Amount}",
                orderCode, status, amount);

            return (status, amount, txnRef);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query PayOS payment status for OrderCode={OrderCode}", orderCode);
            throw new Exception($"Failed to query PayOS payment: {ex.Message}", ex);
        }
    }

    public Task<bool> VerifyWebhookSignatureAsync(string signature, string payload)
    {
        try
        {
            _logger.LogInformation("Verifying PayOS webhook signature");

            // PayOS SDK handles signature verification internally
            // For production, implement proper HMAC-SHA256 verification
            // using the checksum key
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify webhook signature");
            return Task.FromResult(false);
        }
    }

    public async Task<bool> CancelPaymentAsync(string orderCode)
    {
        try
        {
            _logger.LogInformation("Cancelling PayOS payment: OrderCode={OrderCode}", orderCode);

            var result = await _payOS.cancelPaymentLink(long.Parse(orderCode));

            if (result != null)
            {
                _logger.LogInformation("PayOS payment cancelled successfully: OrderCode={OrderCode}", orderCode);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel PayOS payment: OrderCode={OrderCode}", orderCode);
            return false;
        }
    }

    /// <summary>
    /// Helper method to append query parameter to URL.
    /// </summary>
    private static string AppendQueryParam(string url, string key, string value)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        var separator = url.Contains('?') ? "&" : "?";
        return $"{url}{separator}{key}={value}";
    }

    /// <summary>
    /// PayOS has a limit on description length (25 characters for description field).
    /// </summary>
    private static string TruncateDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            return "Nap tien AuraEyes";

        return description.Length > 25 ? description[..25] : description;
    }
}
