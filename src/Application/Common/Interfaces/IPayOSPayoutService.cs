namespace Application.Common.Interfaces;

// ─────────────────────────────────────────────────────────────────────────────
// Models returned by IPayOSPayoutService
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Kết quả lệnh chi PayOS (payout) – ánh xạ từ PayOS Payout API response.
/// </summary>
public class PayOSPayoutResult
{
    /// <summary>PayOS payout ID.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Reference ID do hệ thống sinh ra, gửi lên PayOS.</summary>
    public string ReferenceId { get; set; } = string.Empty;

    /// <summary>Trạng thái phê duyệt: PROCESSING | SUCCEEDED | FAILED.</summary>
    public string ApprovalState { get; set; } = string.Empty;

    /// <summary>Danh mục thanh toán (salary, bonus, …).</summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>Thời điểm PayOS tạo lệnh chi.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>Danh sách giao dịch chi tiết bên trong lệnh chi.</summary>
    public List<PayOSPayoutTransaction> Transactions { get; set; } = new();
}

/// <summary>
/// Giao dịch chi tiết trong lệnh chi PayOS.
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

    /// <summary>Trạng thái: PROCESSING | SUCCEEDED | FAILED.</summary>
    public string State { get; set; } = string.Empty;
}

/// <summary>
/// Kết quả danh sách lệnh chi PayOS (phân trang).
/// </summary>
public class PayOSPayoutListResult
{
    public List<PayOSPayoutResult> Payouts { get; set; } = new();
    public PayOSPayoutPagination Pagination { get; set; } = new();
}

/// <summary>
/// Thông tin phân trang từ PayOS.
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
/// Bộ lọc khi lấy danh sách lệnh chi từ PayOS.
/// </summary>
public class PayOSPayoutFilter
{
    public int Limit { get; set; } = 20;
    public int Offset { get; set; } = 0;
    public string? ReferenceId { get; set; }
    public string? ApprovalState { get; set; }
    public string? Category { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
}

/// <summary>
/// Một mục trong batch payout (dùng cho EstimateCredit).
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
/// Số dư tài khoản chi PayOS.
/// </summary>
public class PayOSPayoutAccountBalance
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Currency { get; set; } = "VND";
    public long Balance { get; set; }
}

// ─────────────────────────────────────────────────────────────────────────────
// Interface
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Service giao tiếp với PayOS Payout API để thực hiện lệnh chi tự động.
/// Base URL: https://api-merchant.payos.vn
/// </summary>
public interface IPayOSPayoutService
{
    /// <summary>
    /// Tạo lệnh chi đơn lẻ qua PayOS Payout API.
    /// </summary>
    /// <param name="referenceId">Mã tham chiếu duy nhất của hệ thống (ví dụ: payout_{withdrawalRequestId}).</param>
    /// <param name="amountVnd">Số tiền VND cần chuyển.</param>
    /// <param name="description">Nội dung chuyển khoản (tối đa 25 ký tự, không dấu).</param>
    /// <param name="toBin">Mã BIN ngân hàng đích (ví dụ: "970415" = Vietinbank).</param>
    /// <param name="toAccountNumber">Số tài khoản ngân hàng đích.</param>
    /// <param name="categories">Danh mục thanh toán (mặc định: ["salary"]).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PayOSPayoutResult> CreatePayoutAsync(
        string referenceId,
        decimal amountVnd,
        string description,
        string toBin,
        string toAccountNumber,
        IEnumerable<string>? categories = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy chi tiết một lệnh chi từ PayOS theo ID.
    /// </summary>
    /// <param name="payoutId">PayOS payout ID (trả về khi tạo lệnh chi).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PayOSPayoutResult> GetPayoutAsync(
        string payoutId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách lệnh chi từ PayOS (có phân trang và lọc).
    /// </summary>
    Task<PayOSPayoutListResult> GetPayoutsAsync(
        PayOSPayoutFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ước tính số dư credit cần thiết cho một batch payout.
    /// </summary>
    Task<long> EstimateCreditAsync(
        string referenceId,
        IEnumerable<string> categories,
        IEnumerable<PayOSPayoutItem> payouts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy số dư tài khoản chi PayOS.
    /// </summary>
    Task<PayOSPayoutAccountBalance> GetPayoutAccountBalanceAsync(
        CancellationToken cancellationToken = default);
}
