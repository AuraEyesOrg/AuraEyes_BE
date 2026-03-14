using Application.AiQuota.Commands.DeductQuota;
using Application.AiQuota.Queries.GetAiQuota;
using Application.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// AI Quota endpoints.
/// Get remaining quota and deduct credits before AI screening.
/// </summary>
[Route("api/quota")]
[Authorize(Policy = Policies.Authenticated)]
public class QuotaController : BaseApiController
{
    private readonly IMediator _mediator;

    public QuotaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get current user's AI screening quota.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetQuota(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAiQuotaQuery(), cancellationToken);
        return HandleResult(result, "AI quota retrieved successfully.");
    }

    /// <summary>
    /// Check and deduct 1 AI screening credit.
    /// Returns 402 Payment Required if quota is exhausted.
    /// </summary>
    [HttpPost("deduct")]
    public async Task<IActionResult> DeductQuota(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeductQuotaCommand(), cancellationToken);
        return HandleResult(result, "Quota deducted successfully.");
    }
}
