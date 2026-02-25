using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Sample controller demonstrating role-based and policy-based authorization.
/// </summary>
public class SampleController : BaseApiController
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<SampleController> _logger;

    public SampleController(
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ILogger<SampleController> logger)
    {
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// S3 connectivity test — uploads a tiny text file and returns the public URL.
    /// Call GET /api/sample/test-s3 to diagnose Supabase S3 issues.
    /// </summary>
    [HttpGet("test-s3")]
    [AllowAnonymous]
    public async Task<IActionResult> TestS3Async(CancellationToken cancellationToken)
    {
        try
        {
            const string testContent = "Supabase S3 connectivity test — AURA";
            var bytes = System.Text.Encoding.UTF8.GetBytes(testContent);
            await using var stream = new MemoryStream(bytes);

            var url = await _fileStorageService.SaveFileAsync(
                stream, "s3-test.txt", "diagnostics", cancellationToken);

            _logger.LogInformation("S3 test upload succeeded: {Url}", url);
            return OkResponse(new { success = true, url, timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "S3 test upload failed");
            return ErrorResponse($"S3 test FAILED: {ex.Message}");
        }
    }

    /// <summary>
    /// Public endpoint - no authentication required.
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicEndpoint()
    {
        return OkResponse(new { message = "This is a public endpoint", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Authenticated endpoint - requires valid JWT token.
    /// </summary>
    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult AuthenticatedEndpoint()
    {
        return OkResponse(new
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
        return OkResponse(new
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
        return OkResponse(new
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
        return OkResponse(new
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
        return OkResponse(new
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
        return OkResponse(new
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
        return OkResponse(new
        {
            message = "You have one of the allowed roles!",
            userId = _currentUserService.UserId,
            roles = _currentUserService.Roles
        });
    }
}
