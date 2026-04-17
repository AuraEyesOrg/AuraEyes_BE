using System.Security.Claims;
using Application.Auth.Queries.GetProfileClaimsByUserId;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Screenings.Commands.CompleteAiScreening;
using Application.Wallets.Commands.VerifyPayment;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Services.Testing;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/test-backdoor")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TestBackdoorController : ControllerBase
{
    private const string BackdoorHeaderName = "X-Test-Key";

    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<TestBackdoorController> _logger;
    private readonly IMediator _mediator;
    private readonly IPayOSService _payOSService;
    private readonly ITokenService _tokenService;

    public TestBackdoorController(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<TestBackdoorController> logger,
        IMediator mediator,
        IPayOSService payOSService,
        ITokenService tokenService)
    {
        _environment = environment;
        _configuration = configuration;
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _mediator = mediator;
        _payOSService = payOSService;
        _tokenService = tokenService;
    }

    [HttpPost("reset-and-seed")]
    public async Task<IActionResult> ResetAndSeed(CancellationToken cancellationToken)
    {
        if (!ValidateBackdoorAccess())
            return Forbid();

        await _dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await _dbContext.Database.MigrateAsync(cancellationToken);

        var seederLogger = HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseSeeder");

        await DatabaseSeeder.SeedAsync(_dbContext, _userManager, _roleManager, _configuration, seederLogger);

        _logger.LogInformation("[TEST BACKDOOR] Database reset and seed completed.");

        return Ok(new { success = true, message = "Database reset-and-seed completed." });
    }

    [HttpGet("email/verify-token")]
    public async Task<IActionResult> GetVerifyToken([FromQuery] string email)
    {
        if (!ValidateBackdoorAccess())
            return Forbid();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound(new { success = false, message = "User not found." });

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encoded = Uri.EscapeDataString(token);

        return Ok(new
        {
            success = true,
            userId = user.Id,
            email = user.Email,
            token,
            encodedToken = encoded
        });
    }

    [HttpGet("auth/magic-login")]
    public async Task<IActionResult> MagicLogin([FromQuery] string email, CancellationToken cancellationToken)
    {
        if (!ValidateBackdoorAccess())
            return Forbid();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { success = false, message = "Email is required." });

        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user is null)
            return NotFound(new { success = false, message = "User not found." });

        if (!user.IsActive || user.IsDeleted)
            return BadRequest(new { success = false, message = "User is inactive or deleted." });

        var roles = await _userManager.GetRolesAsync(user);
        var additionalClaims = await BuildProfileClaimsAsync(user.Id, roles, cancellationToken);

        var tokenResult = await _tokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            additionalClaims);

        return Ok(new
        {
            success = true,
            tokenType = "Bearer",
            accessToken = tokenResult.AccessToken,
            expiresAt = tokenResult.ExpiresAt,
            user = new
            {
                id = user.Id,
                email = user.Email,
                fullName = user.FullName,
                roles
            }
        });
    }

    [HttpPost("email/confirm")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailBypassRequest request)
    {
        if (!ValidateBackdoorAccess())
            return Forbid();

        ApplicationUser? user = null;

        if (!string.IsNullOrWhiteSpace(request.Email))
            user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null && request.UserId.HasValue)
            user = await _userManager.FindByIdAsync(request.UserId.Value.ToString());

        if (user is null)
            return NotFound(new { success = false, message = "User not found." });

        user.EmailConfirmed = true;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            return BadRequest(new { success = false, errors = updateResult.Errors.Select(x => x.Description) });

        return Ok(new { success = true, message = "Email confirmed via backdoor.", userId = user.Id });
    }

    [HttpPost("payments/mark-success")]
    public IActionResult MarkPaymentSuccess([FromBody] MarkPaymentSuccessRequest request)
    {
        if (!ValidateBackdoorAccess())
            return Forbid();

        if (_payOSService is not FakePayOSService fakePayOs)
            return BadRequest(new { success = false, message = "Fake payment gateway is not active in this environment." });

        var marked = fakePayOs.MarkPaid(request.OrderCode, request.TransactionReference);

        if (!marked)
            return NotFound(new { success = false, message = "Order code not found in fake gateway state." });

        return Ok(new { success = true, orderCode = request.OrderCode, message = "Marked as PAID in fake payment gateway." });
    }

    [HttpPost("payments/verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentBypassRequest request, CancellationToken cancellationToken)
    {
        if (!ValidateBackdoorAccess())
            return Forbid();

        var result = await _mediator.Send(new VerifyPaymentCommand
        {
            OrderCode = request.OrderCode,
            UserId = request.UserId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage,
                errors = result.Errors
            });
        }

        return Ok(new { success = true, data = result.Data });
    }

    [HttpPost("screenings/complete-mock-ai")]
    public async Task<IActionResult> CompleteMockAi(
        [FromBody] CompleteMockAiScreeningRequest request,
        CancellationToken cancellationToken)
    {
        if (!ValidateBackdoorAccess())
            return Forbid();

        var result = await _mediator.Send(new CompleteAiScreeningCommand
        {
            ScreeningId = request.ScreeningId,
            ResultStatus = request.ResultStatus,
            RawJsonOutput = request.RawJsonOutput
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage,
                errors = result.Errors
            });
        }

        return Ok(new { success = true, data = result.Data, message = "AI screening completed using mock payload." });
    }

    private bool ValidateBackdoorAccess()
    {
        if (!_environment.IsEnvironment("Test"))
            return false;

        var expectedKey = _configuration["Testing:BackdoorKey"];
        if (string.IsNullOrWhiteSpace(expectedKey))
            return false;

        if (!Request.Headers.TryGetValue(BackdoorHeaderName, out var provided))
            return false;

        return string.Equals(provided.ToString(), expectedKey, StringComparison.Ordinal);
    }

    private async Task<List<Claim>> BuildProfileClaimsAsync(
        Guid userId,
        IList<string> roles,
        CancellationToken cancellationToken)
    {
        var claims = new List<Claim>();

        var result = await _mediator.Send(
            new GetProfileClaimsByUserIdQuery(userId, roles.ToArray()),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null || result.Data.ProfileId is null)
            return claims;

        claims.Add(new Claim("profile_id", result.Data.ProfileId.Value.ToString()));

        if (roles.Contains(Roles.Ophthalmologist))
        {
            if (result.Data.IsVerified.HasValue)
                claims.Add(new Claim("IsVerified", result.Data.IsVerified.Value.ToString()));

            if (!string.IsNullOrWhiteSpace(result.Data.VerificationStatus))
                claims.Add(new Claim("verification_status", result.Data.VerificationStatus));
        }

        return claims;
    }
}

public record ConfirmEmailBypassRequest
{
    public Guid? UserId { get; init; }
    public string? Email { get; init; }
}

public record MarkPaymentSuccessRequest
{
    public string OrderCode { get; init; } = string.Empty;
    public string? TransactionReference { get; init; }
}

public record VerifyPaymentBypassRequest
{
    public string OrderCode { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
}

public record CompleteMockAiScreeningRequest
{
    public Guid ScreeningId { get; init; }
    public string ResultStatus { get; init; } = "Normal";
    public string RawJsonOutput { get; init; } = "{\"source\":\"mock-ai\"}";
}
