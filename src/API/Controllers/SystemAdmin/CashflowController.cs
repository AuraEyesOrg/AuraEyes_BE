using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Cashflow.Queries.GetCashflowTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

namespace API.Controllers.SystemAdmin;

[Route("api/system-admin/[controller]")]
[AuthorizePermission(Permissions.CashflowRead)]
public class CashflowController : BaseApiController
{
    private readonly IMediator _mediator;

    public CashflowController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("transactions")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CashflowTransactionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] string? actorRole = null,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetCashflowTransactionsQuery
        {
            ActorRole = actorRole,
            Status = status,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDirection = sortDirection,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}
