using Application.Financial.Commands.CreateOrder;
using Application.Financial.Commands.HandlePaymentWebhook;
using Application.Financial.Queries.GetOrderById;
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

    [HttpPost("orders")]
    public async Task<ActionResult> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("orders/{id}")]
    public async Task<ActionResult> GetOrder(Guid id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("payos-webhook")]
    public async Task<IActionResult> PayOSWebhook()
    {
        // PayOS sends signature in header
        var signature = Request.Headers["x-signature"].ToString();
        
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        var result = await _mediator.Send(new HandlePaymentWebhookCommand(signature, payload));
        
        if (!result) return BadRequest();
        
        return Ok();
    }
}
