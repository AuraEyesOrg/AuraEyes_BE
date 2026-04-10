using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Commands.ProcessPayoutViaPayOS;
using Application.Wallets.Commands.SyncPayoutStatus;
using Application.Wallets.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// [SystemAdmin] Quản lý lệnh chi qua PayOS Payout API.
/// Cho phép admin trigger lệnh chi tự động và đồng bộ trạng thái từ PayOS.
/// </summary>
[Route("api/admin/payouts")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class PayoutsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PayoutsController> _logger;

    public PayoutsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        ILogger<PayoutsController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// [Admin] Trigger lệnh chi tự động qua PayOS Payout API cho một WithdrawalRequest.
    /// WithdrawalRequest phải ở trạng thái Pending và có BankBin hợp lệ.
    /// </summary>
    /// <param name="withdrawalRequestId">ID của WithdrawalRequest cần xử lý.</param>
    /// <param name="request">Tham số bổ sung (categories, v.v.).</param>
    [HttpPost("withdrawal-requests/{withdrawalRequestId:guid}/process")]
    [ProducesResponseType(typeof(ApiResponse<PayoutViaPayOSResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ProcessPayoutViaPayOS(
        Guid withdrawalRequestId,
        [FromBody] ProcessPayoutRequest? request = null)
    {
        var adminId = _currentUserService.UserId;
        if (!adminId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Admin not authenticated."));

        var command = new ProcessPayoutViaPayOSCommand
        {
            WithdrawalRequestId = withdrawalRequestId,
            RequestedByUserId = adminId.Value,
            Categories = request?.Categories ?? new List<string> { "salary" }
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Admin {AdminId} processed payout for WithdrawalRequest {RequestId} via PayOS. ExternalPayoutId={ExternalId}",
                adminId.Value, withdrawalRequestId, result.Data?.ExternalPayoutId);

            return Ok(ApiResponseFactory.Success(
                result.Data!,
                $"Payout submitted to PayOS successfully. ApprovalState: {result.Data!.ApprovalState}"));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// [Admin] Đồng bộ trạng thái lệnh chi từ PayOS về hệ thống.
    /// Gọi PayOS GET /v1/payouts/{payoutId} và cập nhật WithdrawalRequest.
    /// Sử dụng khi cần force-sync sau khi PayOS xử lý.
    /// </summary>
    /// <param name="withdrawalRequestId">ID của WithdrawalRequest cần đồng bộ.</param>
    [HttpPost("withdrawal-requests/{withdrawalRequestId:guid}/sync-status")]
    [ProducesResponseType(typeof(ApiResponse<PayoutStatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SyncPayoutStatus(Guid withdrawalRequestId)
    {
        var adminId = _currentUserService.UserId;
        if (!adminId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Admin not authenticated."));

        var command = new SyncPayoutStatusCommand
        {
            WithdrawalRequestId = withdrawalRequestId,
            RequestedByUserId = adminId.Value
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Admin {AdminId} synced payout status for WithdrawalRequest {RequestId}. ApprovalState={State}",
                adminId.Value, withdrawalRequestId, result.Data?.ApprovalState);

            return Ok(ApiResponseFactory.Success(
                result.Data!,
                $"Payout status synced from PayOS. ApprovalState: {result.Data!.ApprovalState}"));
        }

        return HandleResult(result);
    }
}

#region Request DTOs

/// <summary>
/// Request body cho ProcessPayoutViaPayOS.
/// </summary>
public class ProcessPayoutRequest
{
    /// <summary>
    /// Danh mục thanh toán PayOS.
    /// Mặc định: ["salary"]. Có thể dùng: salary, bonus, commission.
    /// </summary>
    public List<string> Categories { get; set; } = new() { "salary" };
}

#endregion
