using Application.Common.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OutputCaching;
using Infrastructure.Persistence;

namespace API.Controllers.Guest;

/// <summary>
/// Public endpoints for Guest-facing metrics and data.
/// </summary>
[Route("api/guest")]
[AllowAnonymous]
public class GuestController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GuestController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Gets the total active user count and top 5 recent avatars to build the Social Proof indicator.
    /// This queries directly from AspNetUsers to accurately represent Patient + Orga + Ophtha.
    /// </summary>
    [HttpGet("trusted-users")]
    [AllowAnonymous]
    [OutputCache(PolicyName = "PublicData")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrustedUsersMetrics()
    {
        // Query active and non-deleted users
        var activeUsersQuery = _userManager.Users.Where(u => u.IsActive && !u.IsDeleted);

        var totalCount = await activeUsersQuery.CountAsync();

        // Ensure we only expose avatars and no sensitive data
        var avatars = await activeUsersQuery
            .Where(u => !string.IsNullOrEmpty(u.AvatarUrl))
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => u.AvatarUrl)
            .Take(5)
            .ToListAsync();

        return OkResponse(new
        {
            count = totalCount,
            avatars = avatars
        });
    }

    /// <summary>
    /// Gets aggregated metrics for the guest home page.
    /// </summary>
    [HttpGet("overview-metrics")]
    [AllowAnonymous]
    [OutputCache(PolicyName = "PublicData")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverviewMetrics([FromServices] ApplicationDbContext dbContext)
    {
        var ophthalmologistCount = await dbContext.Ophthalmologists
            .Where(o => o.IsVerified == true)
            .CountAsync();

        var organisationCount = await dbContext.Organisations.CountAsync();

        var screeningCount = await dbContext.AiScreenings.CountAsync();

        var websiteFeedbackCount = await dbContext.WebsiteFeedbacks.CountAsync();
        var orgFeedbackCount = await dbContext.OrganisationFeedbacks.CountAsync();
        var ophthaFeedbackCount = await dbContext.OphthalmologistFeedbacks.CountAsync();
        
        var totalFeedbackCount = websiteFeedbackCount + orgFeedbackCount + ophthaFeedbackCount;

        return OkResponse(new
        {
            ophthalmologistCount = ophthalmologistCount,
            organisationCount = organisationCount,
            screeningCount = screeningCount,
            feedbackCount = totalFeedbackCount
        });
    }
}
