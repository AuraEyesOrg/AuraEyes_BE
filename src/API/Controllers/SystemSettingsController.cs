using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemSettings.ExperiencePricingRules.Commands.UpdateExperiencePricingRulePrices;
using Application.SystemSettings.ExperiencePricingRules.Common;
using Application.SystemSettings.ExperiencePricingRules.Queries.GetExperiencePricingRules;
using Application.SystemSettings.Commands.UpdateSystemSettings;
using Application.SystemSettings.Queries.GetSystemSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using Infrastructure.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/system-settings")]
[AuthorizePermission(Permissions.SettingsRead)]
public class SystemSettingsController : BaseApiController
{
    private readonly ISender _sender;

    public SystemSettingsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all system settings.
    /// Can be accessed by anyone (used by Patient app to get advance booking limit).
    /// </summary>
    [HttpGet]
    [OutputCache(PolicyName = "PublicData")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSystemSettings(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSystemSettingsQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates one or multiple system settings.
    /// Restricted to System Admin role.
    /// </summary>
    [HttpPut]
    [AuthorizePermission(Permissions.SettingsManage)]
    public async Task<IActionResult> UpdateSystemSettings([FromBody] Dictionary<string, string> settings, CancellationToken cancellationToken)
    {
        var command = new UpdateSystemSettingsCommand { Settings = settings };
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Gets experience-based pricing bands for part-time ophthalmologists.
    /// Restricted to System Admin role.
    /// </summary>
    [HttpGet("experience-pricing-rules")]
    [AuthorizePermission(Permissions.SettingsRead)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ExperiencePricingRuleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExperiencePricingRules(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetExperiencePricingRulesQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates min/max prices for configured experience-based pricing bands.
    /// Restricted to System Admin role.
    /// </summary>
    [HttpPut("experience-pricing-rules")]
    [AuthorizePermission(Permissions.SettingsManage)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ExperiencePricingRuleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExperiencePricingRules(
        [FromBody] List<UpdateExperiencePricingRulePriceItem> rules,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExperiencePricingRulePricesCommand
        {
            Rules = rules
        };

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
