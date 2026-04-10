using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// Implementation của PayOS Payout API (chi tiền / disbursement).
/// Sử dụng HttpClient trực tiếp vì PayOS .NET SDK chưa hỗ trợ Payout API.
/// Base URL: https://api-merchant.payos.vn
/// </summary>
public class PayOSPayoutService : IPayOSPayoutService
{
    private readonly HttpClient _httpClient;
    private readonly PayOSSettings _settings;
    private readonly ILogger<PayOSPayoutService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public PayOSPayoutService(
        HttpClient httpClient,
        IOptions<PayOSSettings> settings,
        ILogger<PayOSPayoutService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        // Set base address and common headers
        _httpClient.BaseAddress = new Uri("https://api-merchant.payos.vn");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("x-client-id", _settings.ClientId);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
    }

    /// <inheritdoc/>
    public async Task<PayOSPayoutResult> CreatePayoutAsync(
        string referenceId,
        decimal amountVnd,
        string description,
        string toBin,
        string toAccountNumber,
        IEnumerable<string>? categories = null,
        CancellationToken cancellationToken = default)
    {
        var categoryList = categories?.ToList() ?? new List<string> { "salary" };

        var payload = new
        {
            referenceId,
            amount = (long)amountVnd,
            description,
            toBin,
            toAccountNumber,
            category = categoryList
        };

        var jsonBody = JsonSerializer.Serialize(payload, JsonOptions);
        var idempotencyKey = GenerateIdempotencyKey(referenceId);
        var signature = GenerateSignature(jsonBody);

        _logger.LogInformation(
            "Creating PayOS payout: ReferenceId={ReferenceId}, Amount={Amount}, ToBin={ToBin}, Account={Account}",
            referenceId, amountVnd, toBin, MaskAccountNumber(toAccountNumber));

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/payouts");
        request.Headers.Add("x-idempotency-key", idempotencyKey);
        request.Headers.Add("x-signature", signature);
        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogInformation(
            "PayOS payout response: StatusCode={StatusCode}, Body={Body}",
            (int)response.StatusCode, responseContent);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<PayOSPayoutApiResponse>(responseContent, JsonOptions)
            ?? throw new InvalidOperationException("PayOS returned empty payout response");

        if (result.Code != "00")
            throw new InvalidOperationException($"PayOS payout failed: {result.Desc} (code: {result.Code})");

        return MapToPayoutResult(result.Data!);
    }

    /// <inheritdoc/>
    public async Task<PayOSPayoutResult> GetPayoutAsync(
        string payoutId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting PayOS payout detail: PayoutId={PayoutId}", payoutId);

        var response = await _httpClient.GetAsync($"/v1/payouts/{payoutId}", cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<PayOSPayoutApiResponse>(responseContent, JsonOptions)
            ?? throw new InvalidOperationException("PayOS returned empty payout detail response");

        if (result.Code != "00")
            throw new InvalidOperationException($"PayOS get payout failed: {result.Desc} (code: {result.Code})");

        return MapToPayoutResult(result.Data!);
    }

    /// <inheritdoc/>
    public async Task<PayOSPayoutListResult> GetPayoutsAsync(
        PayOSPayoutFilter filter,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryString(filter);
        var url = $"/v1/payouts{queryParams}";

        _logger.LogInformation("Getting PayOS payouts list: Url={Url}", url);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<PayOSPayoutListApiResponse>(responseContent, JsonOptions)
            ?? throw new InvalidOperationException("PayOS returned empty payout list response");

        if (result.Code != "00")
            throw new InvalidOperationException($"PayOS get payouts failed: {result.Desc} (code: {result.Code})");

        var payouts = result.Data?.Payouts?.Select(MapToPayoutResult).ToList() ?? new List<PayOSPayoutResult>();
        var pagination = result.Data?.Pagination;

        return new PayOSPayoutListResult
        {
            Payouts = payouts,
            Pagination = new PayOSPayoutPagination
            {
                Total = pagination?.Total ?? 0,
                Limit = pagination?.Limit ?? filter.Limit,
                Offset = pagination?.Offset ?? filter.Offset,
                Count = pagination?.Count ?? payouts.Count,
                HasMore = pagination?.HasMore ?? false
            }
        };
    }

    /// <inheritdoc/>
    public async Task<long> EstimateCreditAsync(
        string referenceId,
        IEnumerable<string> categories,
        IEnumerable<PayOSPayoutItem> payouts,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            referenceId,
            category = categories.ToList(),
            validateDestination = true,
            payouts = payouts.Select(p => new
            {
                referenceId = p.ReferenceId,
                amount = p.Amount,
                description = p.Description,
                toBin = p.ToBin,
                toAccountNumber = p.ToAccountNumber
            }).ToList()
        };

        var jsonBody = JsonSerializer.Serialize(payload, JsonOptions);
        var signature = GenerateSignature(jsonBody);

        _logger.LogInformation("Estimating PayOS payout credit: ReferenceId={ReferenceId}", referenceId);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/payouts/estimate-credit");
        request.Headers.Add("x-signature", signature);
        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<PayOSEstimateCreditResponse>(responseContent, JsonOptions)
            ?? throw new InvalidOperationException("PayOS returned empty estimate credit response");

        if (result.Code != "00")
            throw new InvalidOperationException($"PayOS estimate credit failed: {result.Desc} (code: {result.Code})");

        return result.Data?.EstimateCredit ?? 0;
    }

    /// <inheritdoc/>
    public async Task<PayOSPayoutAccountBalance> GetPayoutAccountBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting PayOS payout account balance");

        var response = await _httpClient.GetAsync("/v1/payouts-account/balance", cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<PayOSAccountBalanceResponse>(responseContent, JsonOptions)
            ?? throw new InvalidOperationException("PayOS returned empty balance response");

        if (result.Code != "00")
            throw new InvalidOperationException($"PayOS get balance failed: {result.Desc} (code: {result.Code})");

        return new PayOSPayoutAccountBalance
        {
            AccountNumber = result.Data?.AccountNumber ?? string.Empty,
            AccountName = result.Data?.AccountName ?? string.Empty,
            Currency = result.Data?.Currency ?? "VND",
            Balance = result.Data?.Balance ?? 0
        };
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Sinh idempotency key dựa trên referenceId + timestamp để đảm bảo tính duy nhất.
    /// </summary>
    private static string GenerateIdempotencyKey(string referenceId)
        => $"{referenceId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    /// <summary>
    /// Sinh chữ ký HMAC-SHA256 từ body request và ChecksumKey.
    /// </summary>
    private string GenerateSignature(string jsonBody)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_settings.ChecksumKey);
        var bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
        var hash = HMACSHA256.HashData(keyBytes, bodyBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string BuildQueryString(PayOSPayoutFilter filter)
    {
        var parts = new List<string>
        {
            $"limit={filter.Limit}",
            $"offset={filter.Offset}"
        };

        if (!string.IsNullOrWhiteSpace(filter.ReferenceId))
            parts.Add($"referenceId={Uri.EscapeDataString(filter.ReferenceId)}");
        if (!string.IsNullOrWhiteSpace(filter.ApprovalState))
            parts.Add($"approvalState={Uri.EscapeDataString(filter.ApprovalState)}");
        if (!string.IsNullOrWhiteSpace(filter.Category))
            parts.Add($"category={Uri.EscapeDataString(filter.Category)}");
        if (!string.IsNullOrWhiteSpace(filter.FromDate))
            parts.Add($"fromDate={Uri.EscapeDataString(filter.FromDate)}");
        if (!string.IsNullOrWhiteSpace(filter.ToDate))
            parts.Add($"toDate={Uri.EscapeDataString(filter.ToDate)}");

        return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
    }

    private static PayOSPayoutResult MapToPayoutResult(PayOSPayoutData data)
    {
        var result = new PayOSPayoutResult
        {
            Id = data.Id ?? string.Empty,
            ReferenceId = data.ReferenceId ?? string.Empty,
            ApprovalState = data.ApprovalState ?? string.Empty,
            Categories = data.Category ?? new List<string>(),
            CreatedAt = data.CreatedAt
        };

        if (data.Transactions != null)
        {
            result.Transactions = data.Transactions.Select(t => new PayOSPayoutTransaction
            {
                Id = t.Id ?? string.Empty,
                ReferenceId = t.ReferenceId ?? string.Empty,
                Amount = t.Amount,
                Description = t.Description ?? string.Empty,
                ToBin = t.ToBin ?? string.Empty,
                ToAccountNumber = t.ToAccountNumber ?? string.Empty,
                ToAccountName = t.ToAccountName ?? string.Empty,
                State = t.State ?? string.Empty
            }).ToList();
        }

        return result;
    }

    private static string MaskAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length <= 4)
            return "****";
        return string.Concat(new string('*', accountNumber.Length - 4), accountNumber.AsSpan(accountNumber.Length - 4));
    }
}

// -------------------------------------------------------------------------
// Internal DTOs for JSON deserialization (PayOS API response shapes)
// -------------------------------------------------------------------------

internal class PayOSPayoutApiResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public PayOSPayoutData? Data { get; set; }
}

internal class PayOSPayoutListApiResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public PayOSPayoutListData? Data { get; set; }
}

internal class PayOSPayoutListData
{
    [JsonPropertyName("payouts")]
    public List<PayOSPayoutData>? Payouts { get; set; }

    [JsonPropertyName("pagination")]
    public PayOSPaginationData? Pagination { get; set; }
}

internal class PayOSPaginationData
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }
}

internal class PayOSPayoutData
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("referenceId")]
    public string? ReferenceId { get; set; }

    [JsonPropertyName("transactions")]
    public List<PayOSTransactionData>? Transactions { get; set; }

    [JsonPropertyName("category")]
    public List<string>? Category { get; set; }

    [JsonPropertyName("approvalState")]
    public string? ApprovalState { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
}

internal class PayOSTransactionData
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("referenceId")]
    public string? ReferenceId { get; set; }

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("toBin")]
    public string? ToBin { get; set; }

    [JsonPropertyName("toAccountNumber")]
    public string? ToAccountNumber { get; set; }

    [JsonPropertyName("toAccountName")]
    public string? ToAccountName { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }
}

internal class PayOSEstimateCreditResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public PayOSEstimateCreditData? Data { get; set; }
}

internal class PayOSEstimateCreditData
{
    [JsonPropertyName("estimateCredit")]
    public long EstimateCredit { get; set; }
}

internal class PayOSAccountBalanceResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public PayOSAccountBalanceData? Data { get; set; }
}

internal class PayOSAccountBalanceData
{
    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("accountName")]
    public string? AccountName { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("balance")]
    public long Balance { get; set; }
}
