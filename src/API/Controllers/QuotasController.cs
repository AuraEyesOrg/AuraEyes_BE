using Application.AiQuota.Queries.GetQuotaBalance;
using Application.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/quotas")]
[Authorize(Policy = Policies.Authenticated)]
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
}
