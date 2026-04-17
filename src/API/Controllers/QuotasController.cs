using Application.AiQuota.Commands.BuyAiQuota;
using Application.AiQuota.Queries.GetQuotaBalance;
using Application.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Infrastructure.Identity.Authorization;

namespace API.Controllers;

[Route("api/quotas")]
[AuthorizePermission(Permissions.QuotasRead)]
public class QuotasController : BaseApiController
{
    private readonly IMediator _mediator;

    public QuotasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get current user's AI quota balance.
    /// Patient reads from Patient table, OrgAdmin reads from Organisation/Contract.
    /// </summary>
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetQuotaBalanceQuery(), cancellationToken);
        return HandleResult(result, "Quota balance retrieved successfully.");
    }

    /// <summary>
    /// Buy AI quota amount using internal wallet balance.
    /// Deducts money from wallet and adds quota credits.
    /// </summary>
    [HttpPost("buy")]
    [AuthorizePermission(Permissions.QuotasBuy)]
    public async Task<IActionResult> BuyQuota(
        [FromBody] BuyAiQuotaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BuyAiQuotaCommand
        {
            QuotaAmount = request.QuotaAmount
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "AI quota purchased successfully.");
    }
}

public record BuyAiQuotaRequest
{
    public int QuotaAmount { get; init; } = 5;
}
