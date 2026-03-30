using Application.Common.Constants;
using Application.Common.Models;
using Application.Consents.Commands.AgreeScreeningConsent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Consent management endpoints.
/// </summary>
[Route("api/consents")]
[Authorize(Policy = Policies.PatientOnly)]
public class ConsentsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ConsentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Agree consent for a screening session and persist legal text content.
    /// </summary>
    [HttpPost("screenings/{screeningId:guid}/agree")]
    [ProducesResponseType(typeof(ApiResponse<AgreeScreeningConsentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AgreeScreeningConsent(
        [FromRoute] Guid screeningId,
        [FromBody] AgreeScreeningConsentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AgreeScreeningConsentCommand
        {
            ScreeningId = screeningId,
            Content = request.Content
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Consent saved successfully");
    }
}

public record AgreeScreeningConsentRequest
{
    public string Content { get; init; } = string.Empty;
}
