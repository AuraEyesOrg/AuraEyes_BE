using System.Collections.Concurrent;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Testing;

/// <summary>
/// Test double for PayOS. Keeps payment state in memory.
/// </summary>
public sealed class FakePayOSService : IPayOSService
{
    private readonly ILogger<FakePayOSService> _logger;

    private static readonly ConcurrentDictionary<string, FakePaymentState> Payments = new();

    public FakePayOSService(ILogger<FakePayOSService> logger)
    {
        _logger = logger;
    }

    public Task<(string PaymentUrl, string OrderCode)> CreatePaymentLinkAsync(
        Guid depositRequestId,
        decimal amountVnd,
        string description,
        string returnUrl,
        string cancelUrl)
    {
        var orderCode = Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();

        Payments[orderCode] = new FakePaymentState
        {
            Status = "PENDING",
            Amount = amountVnd,
            TxnRef = string.Empty
        };

        var separator = returnUrl.Contains('?') ? "&" : "?";
        var paymentUrl = $"{returnUrl}{separator}orderCode={orderCode}&provider=fake-payos";

        _logger.LogInformation(
            "[FAKE PAYOS] Created fake payment link. OrderCode={OrderCode}, Amount={Amount}",
            orderCode,
            amountVnd);

        return Task.FromResult((paymentUrl, orderCode));
    }

    public Task<(string Status, decimal Amount, string TxnRef)> GetPaymentStatusAsync(string orderCode)
    {
        if (Payments.TryGetValue(orderCode, out var state))
        {
            return Task.FromResult((state.Status, state.Amount, state.TxnRef));
        }

        _logger.LogWarning("[FAKE PAYOS] Unknown order code requested: {OrderCode}", orderCode);
        return Task.FromResult(("PENDING", 0m, string.Empty));
    }

    public Task<bool> VerifyWebhookSignatureAsync(string signature, string payload)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CancelPaymentAsync(string orderCode)
    {
        if (!Payments.TryGetValue(orderCode, out var state))
        {
            return Task.FromResult(false);
        }

        state.Status = "CANCELLED";
        return Task.FromResult(true);
    }

    public bool MarkPaid(string orderCode, string? txnRef = null)
    {
        if (!Payments.TryGetValue(orderCode, out var state))
        {
            return false;
        }

        state.Status = "PAID";
        state.TxnRef = string.IsNullOrWhiteSpace(txnRef)
            ? $"FAKE-TXN-{DateTime.UtcNow:yyyyMMddHHmmss}"
            : txnRef;

        _logger.LogInformation("[FAKE PAYOS] Marked payment as PAID. OrderCode={OrderCode}", orderCode);
        return true;
    }

    private sealed class FakePaymentState
    {
        public string Status { get; set; } = "PENDING";
        public decimal Amount { get; set; }
        public string TxnRef { get; set; } = string.Empty;
    }
}
