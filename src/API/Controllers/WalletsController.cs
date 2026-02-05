using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Commands.CreateDeposit;
using Application.Wallets.Commands.VerifyPayment;
using Application.Wallets.Common;
using Application.Wallets.Queries.GetDepositHistory;
using Application.Wallets.Queries.GetDepositRequest;
using Application.Wallets.Queries.GetWallet;
using Application.Wallets.Queries.GetWalletTransactions;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Wallet management endpoints.
/// Provides wallet operations including deposits via PayOS.
/// </summary>
public class WalletsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<WalletsController> _logger;

    public WalletsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        ILogger<WalletsController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's wallet information.
    /// </summary>
    /// <returns>Wallet details including balance.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WalletDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWallet()
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var result = await _mediator.Send(new GetWalletQuery(userId.Value));
        return HandleResult(result);
    }

    /// <summary>
    /// Get wallet transaction history with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 20).</param>
    /// <returns>Paginated list of wallet transactions.</returns>
    [HttpGet("transactions")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<WalletTransactionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var query = new GetWalletTransactionsQuery
        {
            UserId = userId.Value,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get deposit history with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 20).</param>
    /// <returns>Paginated list of deposit requests.</returns>
    [HttpGet("deposits")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DepositRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDepositHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var query = new GetDepositHistoryQuery
        {
            UserId = userId.Value,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific deposit request by ID.
    /// </summary>
    /// <param name="id">Deposit request ID.</param>
    /// <returns>Deposit request details.</returns>
    [HttpGet("deposits/{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<DepositRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDepositRequest(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var result = await _mediator.Send(new GetDepositRequestQuery(id));
        
        // Verify user owns this deposit request
        if (result.IsSuccess && result.Data!.UserId != userId.Value)
        {
            return StatusCode(403, ApiResponseFactory.Forbidden("You are not authorized to view this deposit request."));
        }
        
        return HandleResult(result);
    }

    /// <summary>
    /// Create a deposit request and get PayOS payment link.
    /// </summary>
    /// <param name="request">Deposit request details.</param>
    /// <returns>Payment link and deposit request information.</returns>
    [HttpPost("deposit")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CreateDepositResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateDeposit([FromBody] CreateDepositRequest request)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var command = new CreateDepositCommand
        {
            UserId = userId.Value,
            AmountVnd = request.AmountVnd,
            PaymentMethod = request.PaymentMethod ?? PaymentMethod.PayOS,
            Description = request.Description,
            ReturnUrl = request.ReturnUrl,
            CancelUrl = request.CancelUrl
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(ApiResponseFactory.Success(result.Data!, "Deposit request created successfully. Please complete the payment."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Verify payment status and credit wallet if successful.
    /// </summary>
    /// <param name="request">Verify payment request.</param>
    /// <returns>Payment verification result.</returns>
    [HttpPost("verify-payment")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<VerifyPaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var command = new VerifyPaymentCommand
        {
            OrderCode = request.OrderCode,
            UserId = userId.Value
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get payment status (public endpoint for checking without auth).
    /// </summary>
    /// <param name="orderCode">PayOS order code.</param>
    /// <returns>Payment status information.</returns>
    [HttpGet("payment-status/{orderCode}")]
    [ProducesResponseType(typeof(ApiResponse<VerifyPaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentStatus(string orderCode)
    {
        var command = new VerifyPaymentCommand
        {
            OrderCode = orderCode,
            UserId = null // No user validation for status check
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// PayOS Webhook endpoint for payment notifications.
    /// </summary>
    /// <param name="request">PayOS webhook payload.</param>
    /// <returns>Acknowledgment response.</returns>
    [HttpPost("webhook/payos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PayOSWebhook([FromBody] PayOSWebhookRequest request)
    {
        _logger.LogInformation(
            "Received PayOS webhook: Code={Code}, Success={Success}, OrderCode={OrderCode}",
            request.Code, request.Success, request.Data?.OrderCode);

        try
        {
            if (!request.Success || request.Data == null)
            {
                _logger.LogWarning("PayOS webhook indicates failure or no data");
                return Ok(new { success = true, message = "Webhook received" });
            }

            // Verify payment and credit wallet
            var command = new VerifyPaymentCommand
            {
                OrderCode = request.Data.OrderCode.ToString()
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "PayOS webhook processed successfully for OrderCode: {OrderCode}",
                    request.Data.OrderCode);
            }
            else
            {
                _logger.LogWarning("Failed to process PayOS webhook: {Error}", result.ErrorMessage);
            }

            // Always return 200 to acknowledge webhook receipt
            return Ok(new { success = true, message = "Webhook processed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PayOS webhook");
            return Ok(new { success = true, message = "Webhook received with errors" });
        }
    }
}

#region Request DTOs

/// <summary>
/// Request DTO for creating a deposit.
/// </summary>
public class CreateDepositRequest
{
    /// <summary>
    /// Amount in VND (minimum 10,000, maximum 50,000,000).
    /// </summary>
    public decimal AmountVnd { get; set; }

    /// <summary>
    /// Payment method (default: PayOS).
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Description for the deposit.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Return URL after successful payment.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Cancel URL when payment is cancelled.
    /// </summary>
    public string? CancelUrl { get; set; }
}

/// <summary>
/// Request DTO for verifying payment.
/// </summary>
public class VerifyPaymentRequest
{
    /// <summary>
    /// PayOS order code.
    /// </summary>
    public string OrderCode { get; set; } = string.Empty;
}

/// <summary>
/// PayOS webhook request payload.
/// </summary>
public class PayOSWebhookRequest
{
    public string Code { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public bool Success { get; set; }
    public PayOSWebhookData? Data { get; set; }
    public string Signature { get; set; } = string.Empty;
}

/// <summary>
/// PayOS webhook data payload.
/// </summary>
public class PayOSWebhookData
{
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string TransactionDateTime { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string PaymentLinkId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
}

#endregion
