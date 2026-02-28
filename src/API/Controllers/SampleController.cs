using System.Text;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Supabase;

namespace API.Controllers;

/// <summary>
/// Sample controller demonstrating role-based and policy-based authorization.
/// </summary>
public class SampleController : BaseApiController
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<SampleController> _logger;
    private readonly SupabaseStorageSettings _storageSettings;

    public SampleController(
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ILogger<SampleController> logger,
        IOptions<SupabaseStorageSettings> storageSettings)
    {
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _storageSettings = storageSettings.Value;
    }

    /// <summary>
    /// Supabase Storage diagnostic endpoint.
    /// Tests: supabase-csharp SDK upload + IFileStorageService upload.
    /// Call GET /api/sample/test-s3 to diagnose upload issues.
    /// </summary>
    [HttpGet("test-s3")]
    [AllowAnonymous]
    public async Task<IActionResult> TestS3Async(CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, object>
        {
            ["config"] = new
            {
                url = _storageSettings.Url,
                bucket = _storageSettings.BucketName,
                hasServiceKey = !string.IsNullOrEmpty(_storageSettings.ServiceKey),
                utcNow = DateTime.UtcNow.ToString("O")
            }
        };

        // ── 1. Direct supabase-csharp SDK test ──
        try
        {
            var supabase = new Client(_storageSettings.Url, _storageSettings.ServiceKey, new SupabaseOptions
            {
                AutoConnectRealtime = false
            });
            await supabase.InitializeAsync();

            var testBytes = Encoding.UTF8.GetBytes($"Supabase SDK test – {DateTime.UtcNow:O}");
            var testPath = $"diagnostics/sdk-test-{DateTime.UtcNow:yyyyMMdd-HHmmss}.txt";

            await supabase.Storage
                .From(_storageSettings.BucketName)
                .Upload(testBytes, testPath, new Supabase.Storage.FileOptions
                {
                    ContentType = "text/plain",
                    Upsert = true
                });

            var publicUrl = supabase.Storage
                .From(_storageSettings.BucketName)
                .GetPublicUrl(testPath);

            results["1_sdkUpload"] = new
            {
                success = true,
                path = testPath,
                publicUrl
            };
        }
        catch (Exception ex)
        {
            results["1_sdkUpload"] = new
            {
                success = false,
                message = ex.Message,
                innerMessage = ex.InnerException?.Message
            };
        }

        // ── 2. IFileStorageService test (uses SupabaseStorageService) ──
        try
        {
            var bytes = Encoding.UTF8.GetBytes("IFileStorageService test");
            await using var stream = new MemoryStream(bytes);
            var url = await _fileStorageService.SaveFileAsync(
                stream, "service-test.txt", "diagnostics", cancellationToken);

            results["2_serviceUpload"] = new { success = true, url };
        }
        catch (Exception ex)
        {
            results["2_serviceUpload"] = new
            {
                success = false,
                message = ex.Message,
                innerMessage = ex.InnerException?.Message
            };
        }

        _logger.LogInformation("Supabase Storage diagnostic: {@Results}", results);
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
