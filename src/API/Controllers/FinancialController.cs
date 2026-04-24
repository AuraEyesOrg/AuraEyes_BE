using Application.Financial.Commands.CreateOrder;
using Application.Financial.Commands.HandlePaymentWebhook;
using Application.Financial.Queries.GetOrderById;
using Application.Financial.Queries.GetUserOrders;
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
    public async Task<ActionResult> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Returns a single order with its payments by ID.
    /// </summary>
    [HttpGet("orders/{id}")]
    public async Task<ActionResult> GetOrder(Guid id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

  /// <summary>
  /// Returns the paginated payment-order history for the currently authenticated user.
  /// Used by the patient "Ví / Payment History" page.
  /// Route: GET api/financial/my-orders (avoids conflict with /orders/{id:guid})
  /// </summary>
  [HttpGet("my-orders")]
  public async Task<ActionResult> GetMyOrders(
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 20)
  {
      var result = await _mediator.Send(new GetUserOrdersQuery(pageNumber, pageSize));
      return Ok(result);
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
