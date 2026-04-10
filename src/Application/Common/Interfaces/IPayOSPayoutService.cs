namespace Application.Common.Interfaces;

/// <summary>
/// PayOS Payout Service - tạo và quản lý lệnh chi (disbursement) qua PayOS Payout API.
/// Tài liệu: POST /v1/payouts, GET /v1/payouts, GET /v1/payouts/{payoutId}
/// </summary>
public interface IPayOSPayoutService
{
    /// <summary>
    /// Tạo một lệnh chi đơn lẻ qua PayOS Payout API (POST /v1/payouts).
    /// </summary>
    /// <param name="referenceId">Mã tham chiếu nội bộ (unique per request).</param>
    /// <param name="amountVnd">Số tiền thanh toán (VND).</param>
    /// <param name="description">Mô tả thanh toán.</param>
    /// <param name="toBin">Mã ngân hàng đích (BIN).</param>
    /// <param name="toAccountNumber">Số tài khoản ngân hàng đích.</param>
    /// <param name="categories">Danh mục thanh toán (e.g. ["salary"]).</param>
    /// <returns>Thông tin lệnh chi PayOS.</returns>
    Task<PayOSPayoutResult> CreatePayoutAsync(
        string referenceId,
        decimal amountVnd,
        string description,
        string toBin,
        string toAccountNumber,
        IEnumerable<string>? categories = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy thông tin chi tiết một lệnh chi theo PayOS payout ID (GET /v1/payouts/{payoutId}).
    /// </summary>
    /// <param name="payoutId">ID lệnh chi PayOS.</param>
    /// <returns>Thông tin lệnh chi.</returns>
    Task<PayOSPayoutResult> GetPayoutAsync(
        string payoutId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách lệnh chi với bộ lọc tùy chọn (GET /v1/payouts).
    /// </summary>
    /// <param name="filter">Bộ lọc danh sách lệnh chi.</param>
    /// <returns>Danh sách lệnh chi và thông tin phân trang.</returns>
    Task<PayOSPayoutListResult> GetPayoutsAsync(
        PayOSPayoutFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ước tính phí cho một batch lệnh chi (POST /v1/payouts/estimate-credit).
    /// </summary>
    /// <param name="referenceId">Mã tham chiếu.</param>
    /// <param name="categories">Danh mục thanh toán.</param>
    /// <param name="payouts">Danh sách lệnh chi cần ước tính.</param>
    /// <returns>Số credit ước tính.</returns>
    Task<long> EstimateCreditAsync(
        string referenceId,
        IEnumerable<string> categories,
        IEnumerable<PayOSPayoutItem> payouts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy số dư tài khoản chi PayOS (GET /v1/payouts-account/balance).
    /// </summary>
    /// <returns>Thông tin số dư.</returns>
    Task<PayOSPayoutAccountBalance> GetPayoutAccountBalanceAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Kết quả tạo/lấy lệnh chi từ PayOS.
/// </summary>
public class PayOSPayoutResult
{
    /// <summary>ID lệnh chi (PayOS id field).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Mã tham chiếu nội bộ (referenceId).</summary>
    public string ReferenceId { get; set; } = string.Empty;

    /// <summary>Trạng thái phê duyệt: PROCESSING | SUCCEEDED | FAILED.</summary>
    public string ApprovalState { get; set; } = string.Empty;

    /// <summary>Danh sách giao dịch chi tiết.</summary>
    public List<PayOSPayoutTransaction> Transactions { get; set; } = new();

    /// <summary>Danh mục thanh toán.</summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>Thời điểm tạo.</summary>
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Giao dịch chi tiết bên trong một lệnh chi PayOS.
/// </summary>
public class PayOSPayoutTransaction
{
    public string Id { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ToBin { get; set; } = string.Empty;
    public string ToAccountNumber { get; set; } = string.Empty;
    public string ToAccountName { get; set; } = string.Empty;

    /// <summary>Trạng thái giao dịch: PROCESSING | SUCCEEDED | FAILED.</summary>
    public string State { get; set; } = string.Empty;
}

/// <summary>
/// Kết quả lấy danh sách lệnh chi PayOS.
/// </summary>
public class PayOSPayoutListResult
{
    public List<PayOSPayoutResult> Payouts { get; set; } = new();
    public PayOSPayoutPagination Pagination { get; set; } = new();
}

/// <summary>
/// Thông tin phân trang trả về từ PayOS.
/// </summary>
public class PayOSPayoutPagination
{
    public int Total { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
    public bool HasMore { get; set; }
}

/// <summary>
/// Bộ lọc cho API lấy danh sách lệnh chi.
/// </summary>
public class PayOSPayoutFilter
{
    public int Limit { get; set; } = 10;
    public int Offset { get; set; } = 0;
    public string? ReferenceId { get; set; }
    public string? ApprovalState { get; set; }
    public string? Category { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
}

/// <summary>
/// Một phần tử trong batch lệnh chi (dùng cho estimate-credit và batch payout).
/// </summary>
public class PayOSPayoutItem
{
    public string ReferenceId { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ToBin { get; set; } = string.Empty;
    public string ToAccountNumber { get; set; } = string.Empty;
}

/// <summary>
/// Thông tin số dư tài khoản chi PayOS.
/// </summary>
public class PayOSPayoutAccountBalance
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public long Balance { get; set; }
}
