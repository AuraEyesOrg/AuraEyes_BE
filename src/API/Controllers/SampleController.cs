using Application.Common.Constants;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Sample controller demonstrating role-based and policy-based authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SampleController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SampleController> _logger;

    public SampleController(
        ICurrentUserService currentUserService,
        ILogger<SampleController> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Public endpoint - no authentication required.
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicEndpoint()
    {
        return Ok(new { message = "This is a public endpoint", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Authenticated endpoint - requires valid JWT token.
    /// </summary>
    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult AuthenticatedEndpoint()
    {
        return Ok(new
        {
            message = "You are authenticated!",
            userId = _currentUserService.UserId,
            email = _currentUserService.Email,
            roles = _currentUserService.Roles
        });
    }

    /// <summary>
    /// Patient only endpoint - requires Patient role.
    /// </summary>
    [HttpGet("patient-only")]
    [Authorize(Roles = Roles.Patient)]
    public IActionResult PatientOnlyEndpoint()
    {
        return Ok(new
        {
            message = "Welcome, Patient!",
            userId = _currentUserService.UserId
        });
    }

    /// <summary>
    /// Ophthalmologist only endpoint - requires Ophthalmologist role.
    /// </summary>
    [HttpGet("ophthalmologist-only")]
    [Authorize(Roles = Roles.Ophthalmologist)]
    public IActionResult OphthalmologistOnlyEndpoint()
    {
        return Ok(new
        {
            message = "Welcome, Doctor!",
            userId = _currentUserService.UserId
        });
    }

    /// <summary>
    /// Admin only endpoint - requires OrgAdmin or SystemAdmin role.
    /// Uses policy-based authorization.
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Policy = Policies.AdminsOnly)]
    public IActionResult AdminOnlyEndpoint()
    {
        return Ok(new
        {
            message = "Welcome, Admin!",
            userId = _currentUserService.UserId,
            roles = _currentUserService.Roles
        });
    }

    /// <summary>
    /// System admin only endpoint - requires SystemAdmin role.
    /// </summary>
    [HttpGet("system-admin-only")]
    [Authorize(Policy = Policies.SystemAdminOnly)]
    public IActionResult SystemAdminOnlyEndpoint()
    {
        return Ok(new
        {
            message = "Welcome, System Administrator!",
            userId = _currentUserService.UserId
        });
    }

    /// <summary>
    /// Medical staff endpoint - requires Patient or Ophthalmologist role.
    /// </summary>
    [HttpGet("medical-staff")]
    [Authorize(Policy = Policies.MedicalStaff)]
    public IActionResult MedicalStaffEndpoint()
    {
        return Ok(new
        {
            message = "Welcome, Medical Staff!",
            userId = _currentUserService.UserId,
            roles = _currentUserService.Roles
        });
    }

    /// <summary>
    /// Multi-role endpoint - allows multiple specific roles.
    /// </summary>
    [HttpGet("multi-role")]
    [Authorize(Roles = $"{Roles.Patient},{Roles.Ophthalmologist},{Roles.OrgAdmin}")]
    public IActionResult MultiRoleEndpoint()
    {
        return Ok(new
        {
            message = "You have one of the allowed roles!",
            userId = _currentUserService.UserId,
            roles = _currentUserService.Roles
        });
    }
}
