using System.Text;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Infrastructure.Identity.Authorization;

namespace API.Controllers;

/// <summary>
/// Sample controller demonstrating role-based and policy-based authorization.
/// </summary>
public class SampleController : BaseApiController
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<SampleController> _logger;
    private readonly CloudinarySettings _cloudinarySettings;

    public SampleController(
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ILogger<SampleController> logger,
        IOptions<CloudinarySettings> cloudinarySettings)
    {
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _cloudinarySettings = cloudinarySettings.Value;
    }

    /// <summary>
    /// Cloudinary Storage diagnostic endpoint.
    /// Tests: IFileStorageService upload (now using Cloudinary).
    /// Call GET /api/sample/test-storage to diagnose upload issues.
    /// </summary>
    [HttpGet("test-storage")]
    [AllowAnonymous]
    public async Task<IActionResult> TestStorageAsync(CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, object>
        {
            ["config"] = new
            {
                cloudName = _cloudinarySettings.CloudName,
                folder = _cloudinarySettings.Folder,
                hasApiKey = !string.IsNullOrEmpty(_cloudinarySettings.ApiKey),
                utcNow = DateTime.UtcNow.ToString("O")
            }
        };

        // ── IFileStorageService test ──
        try
        {
            var bytes = Encoding.UTF8.GetBytes($"Cloudinary diagnostic test – {DateTime.UtcNow:O}");
            await using var stream = new MemoryStream(bytes);
            var url = await _fileStorageService.SaveFileAsync(
                stream, "diagnostic-test.txt", "admin/diagnostics", cancellationToken);

            results["upload"] = new { success = true, url };
        }
        catch (Exception ex)
        {
            results["upload"] = new
            {
                success = false,
                message = ex.Message,
                innerMessage = ex.InnerException?.Message
            };
        }

        _logger.LogInformation("Storage diagnostic: {@Results}", results);
        return OkResponse(results);
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
    /// Permission-based endpoint - requires specific permission.
    /// Uses AuthorizePermissionAttribute.
    /// </summary>
    [HttpGet("permission-based")]
    [AuthorizePermission(Permissions.UsersRead)]
    public IActionResult PermissionBasedEndpoint()
    {
        return OkResponse(new
        {
            message = "You have 'users:read' permission!",
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
