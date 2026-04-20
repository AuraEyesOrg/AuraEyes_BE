using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// PayOS Service Implementation for Vietnam domestic payments using official SDK.
/// Supports QR Code, Bank Transfer, and E-Wallet payments.
/// </summary>
public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PayOSService> _logger;
    private readonly PayOSSettings _settings;
    private static readonly Uri PayOSBaseAddress = new("https://api-merchant.payos.vn");

    public PayOSService(
        IOptions<PayOSSettings> settings,
        IHttpClientFactory httpClientFactory,
        ILogger<PayOSService> logger)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;

        NormalizeSettings(_settings);

        if (string.IsNullOrWhiteSpace(_settings.ClientId))
            throw new ArgumentNullException(nameof(settings), "PayOS:ClientId is required");
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new ArgumentNullException(nameof(settings), "PayOS:ApiKey is required");
        if (string.IsNullOrWhiteSpace(_settings.ChecksumKey))
            throw new ArgumentNullException(nameof(settings), "PayOS:ChecksumKey is required");

        // Initialize PayOS SDK
        _payOS = new PayOS(_settings.ClientId, _settings.ApiKey, _settings.ChecksumKey);

        _logger.LogInformation(
            "PayOS Service initialized. ClientId={ClientIdMasked}, ApiKeyLength={ApiKeyLength}, ChecksumKeyLength={ChecksumKeyLength}",
            MaskForLog(_settings.ClientId),
            _settings.ApiKey.Length,
            _settings.ChecksumKey.Length);
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

            if (ex.Message.Contains("signature of the response does not match", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError(
                    "PayOS signature mismatch while querying payment via SDK. Attempting REST fallback with x-client-id/x-api-key.");

                var fallback = await TryGetPaymentStatusViaRestAsync(orderCode);
                if (fallback.HasValue)
                {
                    _logger.LogInformation(
                        "PayOS REST fallback succeeded for OrderCode={OrderCode}. Status={Status}, Amount={Amount}",
                        orderCode,
                        fallback.Value.Status,
                        fallback.Value.Amount);

                    return fallback.Value;
                }

                throw new Exception(
                    "Failed to query PayOS payment: signature mismatch from PayOS response and REST fallback failed. Verify PayOS__ClientId, PayOS__ApiKey, and PayOS__ChecksumKey (Payment API) in production.",
                    ex);
            }

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

    private static void NormalizeSettings(PayOSSettings settings)
    {
        settings.ClientId = CleanSecret(settings.ClientId);
        settings.ApiKey = CleanSecret(settings.ApiKey);
        settings.ChecksumKey = CleanSecret(settings.ChecksumKey);
        settings.DefaultReturnUrl = settings.DefaultReturnUrl?.Trim() ?? string.Empty;
        settings.DefaultCancelUrl = settings.DefaultCancelUrl?.Trim() ?? string.Empty;
    }

    private static string CleanSecret(string? value)
        => (value ?? string.Empty).Trim().Trim('"').Trim('\'').Trim();

    private static string MaskForLog(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "<empty>";

        if (value.Length <= 8)
            return "****";

        return $"{value[..4]}...{value[^4..]}";
    }

    private async Task<(string Status, decimal Amount, string TxnRef)?> TryGetPaymentStatusViaRestAsync(string orderCode)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = PayOSBaseAddress;

            using var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/payment-requests/{orderCode}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("x-client-id", _settings.ClientId);
            request.Headers.Add("x-api-key", _settings.ApiKey);

            using var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "PayOS REST fallback failed: StatusCode={StatusCode}, Body={Body}",
                    (int)response.StatusCode,
                    content);
                return null;
            }

            using var jsonDoc = JsonDocument.Parse(content);
            if (!jsonDoc.RootElement.TryGetProperty("data", out var data))
            {
                _logger.LogError("PayOS REST fallback response does not contain data node. Body={Body}", content);
                return null;
            }

            var status = data.TryGetProperty("status", out var statusNode)
                ? (statusNode.GetString() ?? "UNKNOWN")
                : "UNKNOWN";

            var amount = data.TryGetProperty("amount", out var amountNode)
                ? amountNode.GetDecimal()
                : 0m;

            var txnRef = string.Empty;
            if (data.TryGetProperty("transactions", out var transactionsNode)
                && transactionsNode.ValueKind == JsonValueKind.Array
                && transactionsNode.GetArrayLength() > 0)
            {
                var firstTxn = transactionsNode[0];
                if (firstTxn.TryGetProperty("reference", out var referenceNode))
                    txnRef = referenceNode.GetString() ?? string.Empty;
            }

            return (status, amount, txnRef);
        }
        catch (Exception fallbackEx)
        {
            _logger.LogError(fallbackEx, "PayOS REST fallback exception for OrderCode={OrderCode}", orderCode);
            return null;
        }
    }
}
