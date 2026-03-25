using System.Text.Json;
using Application.Common.Models;
using Application.SystemSettings.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.WebUtilities;

namespace API.Controllers;

[Route("api/patient/resources")]
public class PatientResourcesController : BaseApiController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ISystemSettingService _systemSettingService;
    private readonly ILogger<PatientResourcesController> _logger;

    private const string DefaultTrustedDomains =
        "vinmec.com,vnio.vn,benhvienmat.com,matsaigon.com,matquocte.vn,medlatec.vn,hellobacsi.com";

    public PatientResourcesController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ISystemSettingService systemSettingService,
        ILogger<PatientResourcesController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _systemSettingService = systemSettingService;
        _logger = logger;
    }

    /// <param name="diseases">
    /// Comma-separated list of disease/condition names returned by the AI model
    /// disease is used as the main search keyword.
    /// </param>
    /// <param name="limit">Maximum number of results to return (1–10).</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    [HttpGet("eye-health")]
    [AllowAnonymous]
    [OutputCache(PolicyName = "PublicData")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PatientEducationalResourceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEyeHealthResources(
        [FromQuery] string? diseases = null,
        [FromQuery] int limit = 3,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["SerpApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("SerpApi key is missing. Returning empty educational resource list.");
            return OkResponse<IReadOnlyList<PatientEducationalResourceDto>>(
                Array.Empty<PatientEducationalResourceDto>(),
                "SerpApi key is not configured.");
        }

        // Resolve trusted domains from DB settings; fall back to built-in defaults.
        var rawDomains = await _systemSettingService.GetSettingAsync(
            "TRUSTED_EYE_HEALTH_DOMAINS", cancellationToken);
        if (string.IsNullOrWhiteSpace(rawDomains))
            rawDomains = DefaultTrustedDomains;

        var siteFilter = BuildSiteFilter(rawDomains);
        var searchQuery = BuildSearchQuery(diseases, siteFilter);
        var maxItems = Math.Clamp(limit, 1, 10);

        _logger.LogInformation("SerpApi search query: {Query}", searchQuery);

        var serpBaseUrl = _configuration["SerpApi:BaseUrl"];
        var queryParams = new Dictionary<string, string?>
        {
            { "engine", "google" },
            { "hl", "vi" },
            { "gl", "vn" },
            { "safe", "active" },
            { "num", maxItems.ToString() },
            { "q", searchQuery },
            { "api_key", apiKey }
        };

        var searchUrl = QueryHelpers.AddQueryString(serpBaseUrl, queryParams);

        try
        {
            var client = _httpClientFactory.CreateClient();
            using var response = await client.GetAsync(searchUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "SerpApi request failed with status {StatusCode}.",
                    (int)response.StatusCode);
                return OkResponse<IReadOnlyList<PatientEducationalResourceDto>>(
                    Array.Empty<PatientEducationalResourceDto>(),
                    "Unable to load educational resources at the moment.");
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("organic_results", out var organicResults)
                || organicResults.ValueKind != JsonValueKind.Array)
            {
                return OkResponse<IReadOnlyList<PatientEducationalResourceDto>>(
                    Array.Empty<PatientEducationalResourceDto>(),
                    "No educational resources found.");
            }

            var items = new List<PatientEducationalResourceDto>(maxItems);
            foreach (var result in organicResults.EnumerateArray())
            {
                if (items.Count >= maxItems) break;

                var title = result.TryGetProperty("title", out var titleProp)
                    ? titleProp.GetString()
                    : null;
                var link = result.TryGetProperty("link", out var linkProp)
                    ? linkProp.GetString()
                    : null;

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(link))
                    continue;

                var snippet = result.TryGetProperty("snippet", out var snippetProp)
                    ? snippetProp.GetString()
                    : string.Empty;

                string? image = null;
                if (result.TryGetProperty("thumbnail", out var thumbnailProp))
                    image = thumbnailProp.GetString();
                else if (result.TryGetProperty("favicon", out var faviconProp))
                    image = faviconProp.GetString();

                items.Add(new PatientEducationalResourceDto(
                    Guid.NewGuid().ToString("N"),
                    title,
                    snippet ?? string.Empty,
                    link,
                    image));
            }

            return OkResponse<IReadOnlyList<PatientEducationalResourceDto>>(
                items,
                "Educational resources loaded.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch educational resources from SerpApi.");
            return OkResponse<IReadOnlyList<PatientEducationalResourceDto>>(
                Array.Empty<PatientEducationalResourceDto>(),
                "Unable to load educational resources at the moment.");
        }
    }

    // Builds "(site:vinmec.com OR site:vnio.vn OR ...)" from a comma-separated domain list.
    private static string BuildSiteFilter(string rawDomains)
    {
        var domains = rawDomains.Split(
            ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (domains.Length == 0) return string.Empty;

        var parts = domains.Select(d => $"site:{d.TrimStart('.')}");
        return $"({string.Join(" OR ", parts)})";
    }

    // Combines the primary disease keyword with the site filter.
    private static string BuildSearchQuery(string? diseases, string siteFilter)
    {
        var primaryDisease = string.Empty;

        if (!string.IsNullOrWhiteSpace(diseases))
        {
            primaryDisease = diseases
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault()?.Trim() ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(primaryDisease))
            primaryDisease = "sức khỏe mắt";

        return string.IsNullOrWhiteSpace(siteFilter)
            ? primaryDisease
            : $"{primaryDisease} {siteFilter}";
    }
}

public sealed record PatientEducationalResourceDto(
    string Id,
    string Title,
    string Description,
    string Link,
    string? Image);
