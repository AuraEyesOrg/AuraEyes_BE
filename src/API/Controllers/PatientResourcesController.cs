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
    private const string DefaultHealthKeyword = "suc khoe mat";

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
        var maxItems = Math.Clamp(limit, 1, 10);
        var diseaseTerms = ParseDiseaseTerms(diseases);

        var rawDomains = await _systemSettingService.GetSettingAsync(
            "TRUSTED_EYE_HEALTH_DOMAINS", cancellationToken);
        if (string.IsNullOrWhiteSpace(rawDomains))
            rawDomains = DefaultTrustedDomains;
        var trustedDomains = ParseDomains(rawDomains);
        var serpItems = await FetchSerpEducationalResourcesAsync(
            diseaseTerms,
            trustedDomains,
            maxItems,
            cancellationToken);

        var fallbackItems = BuildMockResources(diseaseTerms, maxItems);
        var merged = MergeWithFallback(serpItems, fallbackItems, maxItems);

        return OkResponse<IReadOnlyList<PatientEducationalResourceDto>>(
            merged,
            serpItems.Count > 0
                ? "Educational resources loaded."
                : "Showing curated educational resources.");
    }

    private async Task<List<PatientEducationalResourceDto>> FetchSerpEducationalResourcesAsync(
        IReadOnlyList<string> diseaseTerms,
        IReadOnlyCollection<string> trustedDomains,
        int maxItems,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["SerpApi:ApiKey"];
        var serpBaseUrl = _configuration["SerpApi:BaseUrl"];
        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(serpBaseUrl))
        {
            _logger.LogWarning("SerpApi credentials are missing, skip real fetch.");
            return [];
        }

        var candidates = new List<PatientEducationalResourceDto>();
        var seenLinks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var searchQueries = BuildSearchQueries(diseaseTerms, trustedDomains);
        var client = _httpClientFactory.CreateClient();

        foreach (var query in searchQueries.Take(4))
        {
            if (candidates.Count >= maxItems) break;
            await FetchAndParseQueryResultsAsync(client, query, serpBaseUrl, apiKey, candidates, seenLinks, trustedDomains, maxItems, cancellationToken);
        }

        return candidates;
    }

    private async Task FetchAndParseQueryResultsAsync(
        HttpClient client,
        string query,
        string serpBaseUrl,
        string apiKey,
        List<PatientEducationalResourceDto> candidates,
        HashSet<string> seenLinks,
        IReadOnlyCollection<string> trustedDomains,
        int maxItems,
        CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "engine", "google" },
            { "hl", "vi" },
            { "gl", "vn" },
            { "safe", "active" },
            { "num", Math.Max(maxItems * 2, 6).ToString() },
            { "q", query },
            { "api_key", apiKey }
        };

        var searchUrl = QueryHelpers.AddQueryString(serpBaseUrl, queryParams);

        try
        {
            using var response = await client.GetAsync(searchUrl, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("SerpApi request failed for query {Query} with status {StatusCode}.", query, (int)response.StatusCode);
                return;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("organic_results", out var organicResults) ||
                organicResults.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            ParseOrganicResults(organicResults, candidates, seenLinks, trustedDomains, maxItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed SerpApi request for query {Query}.", query);
        }
    }

    private static void ParseOrganicResults(
        JsonElement organicResults,
        List<PatientEducationalResourceDto> candidates,
        HashSet<string> seenLinks,
        IReadOnlyCollection<string> trustedDomains,
        int maxItems)
    {
        foreach (var result in organicResults.EnumerateArray())
        {
            if (candidates.Count >= maxItems) break;

            var title = result.TryGetProperty("title", out var titleProp)
                ? titleProp.GetString()
                : null;
            var link = result.TryGetProperty("link", out var linkProp)
                ? linkProp.GetString()
                : null;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(link))
                continue;
            if (!seenLinks.Add(link))
                continue;
            if (!IsAllowedDomain(link, trustedDomains))
                continue;

            var snippet = result.TryGetProperty("snippet", out var snippetProp)
                ? snippetProp.GetString()
                : string.Empty;

            string? image = null;
            if (result.TryGetProperty("thumbnail", out var thumbnailProp))
                image = thumbnailProp.GetString();
            else if (result.TryGetProperty("favicon", out var faviconProp))
                image = faviconProp.GetString();

            candidates.Add(new PatientEducationalResourceDto(
                Guid.NewGuid().ToString("N"),
                title,
                snippet ?? string.Empty,
                link,
                image));
        }
    }

    private static bool IsAllowedDomain(string link, IReadOnlyCollection<string> trustedDomains)
    {
        if (trustedDomains.Count == 0) return true;
        if (!Uri.TryCreate(link, UriKind.Absolute, out var uri)) return false;
        var host = uri.Host.ToLowerInvariant();
        return trustedDomains.Any(domain => host == domain || host.EndsWith("." + domain));
    }

    private static List<string> BuildSearchQueries(
        IReadOnlyList<string> diseaseTerms,
        IReadOnlyCollection<string> trustedDomains)
    {
        var primaryTerm = diseaseTerms.FirstOrDefault() ?? DefaultHealthKeyword;
        var siteFilter = trustedDomains.Count > 0
            ? $"({string.Join(" OR ", trustedDomains.Take(6).Select(d => $"site:{d}"))})"
            : string.Empty;

        var queries = new List<string>
        {
            string.IsNullOrWhiteSpace(siteFilter)
                ? $"{primaryTerm} dieu tri va theo doi"
                : $"{primaryTerm} {siteFilter}",
            $"{primaryTerm} trieu chung va phong ngua",
            $"{primaryTerm} patient education",
            "suc khoe mat phong ngua benh vong mac"
        };

        return queries.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static List<string> ParseDiseaseTerms(string? diseases)
    {
        if (string.IsNullOrWhiteSpace(diseases))
            return [];

        return diseases
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(5)
            .ToList();
    }

    private static List<string> ParseDomains(string rawDomains)
    {
        return rawDomains
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(d => d.Trim().TrimStart('.').ToLowerInvariant())
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<PatientEducationalResourceDto> BuildMockResources(
        IReadOnlyList<string> diseaseTerms,
        int maxItems)
    {
        var primary = diseaseTerms.FirstOrDefault() ?? "suc khoe mat";
        var templates = new List<PatientEducationalResourceDto>
        {
            new(
                $"mock-{Guid.NewGuid():N}",
                $"Huong dan theo doi va cham soc khi co dau hieu {primary}",
                "Tong hop cach theo doi trieu chung va khi nao can di kham chuyen khoa mat.",
                "https://www.nei.nih.gov/learn-about-eye-health",
                null),
            new(
                $"mock-{Guid.NewGuid():N}",
                "Cach chuan bi buoi kham mat sau ket qua AI screening",
                "Danh sach cau hoi nen hoi bac si va thong tin can mang theo khi tai kham.",
                "https://medlineplus.gov/eyediseases.html",
                null),
            new(
                $"mock-{Guid.NewGuid():N}",
                "Tong quan cac benh mat pho bien va cach phong ngua",
                "Tai lieu tong quan de hieu nguy co, trieu chung va huong phong ngua benh ly vong mac.",
                "https://www.who.int/news-room/fact-sheets/detail/blindness-and-vision-impairment",
                null)
        };

        return templates.Take(maxItems).ToList();
    }

    private static List<PatientEducationalResourceDto> MergeWithFallback(
        IReadOnlyList<PatientEducationalResourceDto> serpItems,
        IReadOnlyList<PatientEducationalResourceDto> fallbackItems,
        int maxItems)
    {
        var merged = new List<PatientEducationalResourceDto>(maxItems);
        var seenLinks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in serpItems)
        {
            if (merged.Count >= maxItems) break;
            if (seenLinks.Add(item.Link))
                merged.Add(item);
        }

        foreach (var item in fallbackItems)
        {
            if (merged.Count >= maxItems) break;
            if (seenLinks.Add(item.Link))
                merged.Add(item);
        }

        return merged;
    }
}

public sealed record PatientEducationalResourceDto(
    string Id,
    string Title,
    string Description,
    string Link,
    string? Image);
