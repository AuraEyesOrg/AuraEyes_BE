using Application.Financial.Commands.CreateOrder;
using Application.Financial.Commands.HandlePaymentWebhook;
using Application.Financial.Queries.GetOrderById;
using Application.Financial.Queries.GetUserOrders;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class FinancialController : BaseApiController
{
    private readonly IMediator _mediator;

    public FinancialController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new payment order (generic – e.g., for ad-hoc top-ups).
    /// </summary>
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    /// <summary>
    /// Returns a single order with its payments by ID.
    /// </summary>
    [HttpGet("orders/{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        return result != null ? OkResponse(result) : ErrorResponse("Order not found", 404);
    }

    /// <summary>
    /// Returns the paginated payment-order history for the currently authenticated user.
    /// Used by the patient "Ví / Payment History" page.
    /// </summary>
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetUserOrdersQuery(pageNumber, pageSize));
        return OkResponse(result);
    }

    /// <summary>
    /// Returns all orders in the system (for staff).
    /// </summary>
    [HttpGet("orders/all")]
    [Authorize(Roles = "ClinicStaff,SystemAdmin")]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new Application.Financial.Queries.GetAllOrders.GetAllOrdersQuery(pageNumber, pageSize));
        return OkResponse(result);
    }

    /// <summary>
    /// Completes an order by paying the remaining balance (e.g. at the clinic counter).
    /// </summary>
    [HttpPost("orders/{id}/complete")]
    [Authorize(Roles = "ClinicStaff,SystemAdmin")]
    public async Task<IActionResult> CompleteOrder(Guid id, [FromBody] CompleteOrderRequest request)
    {
        var result = await _mediator.Send(new Application.Financial.Commands.CompleteOrderPayment.CompleteOrderPaymentCommand(
            id, 
            request.Method,
            request.ReturnUrl,
            request.CancelUrl));
            
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    public record CompleteOrderRequest(
        PaymentMethod Method = PaymentMethod.Cash, 
        string? ReturnUrl = null, 
        string? CancelUrl = null);

    /// <summary>
    /// Synchronizes the payment status with PayOS.
    /// </summary>
    [HttpPost("orders/{id}/sync")]
    [Authorize(Roles = "ClinicStaff,SystemAdmin")]
    public async Task<IActionResult> SyncOrderPaymentStatus(Guid id)
    {
        var result = await _mediator.Send(new Application.Financial.Commands.SyncOrderPaymentStatus.SyncOrderPaymentStatusCommand(id));
        return result ? OkResponse("Payment status synced") : ErrorResponse("Order not found", 404);
    }

    /// <summary>
    /// PayOS webhook – receives payment status updates (PAID, CANCELLED, …).
    /// Must be publicly accessible (no auth) since PayOS calls it directly.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("payos-webhook")]
    public async Task<IActionResult> PayOSWebhook()
    {
        var signature = Request.Headers["x-signature"].ToString();

        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        var result = await _mediator.Send(new HandlePaymentWebhookCommand(signature, payload));

        if (!result) return BadRequest();

        return Ok();
    }
}
