using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public sealed class AiAssetBaseUrlProvider : IAiAssetBaseUrlProvider
{
    private static readonly Uri DefaultBaseUri = new("https://ai");

    public Uri BaseUri { get; }

    public AiAssetBaseUrlProvider(IConfiguration configuration)
    {
        var configuredBaseUrl = configuration["AiAsset:BaseUrl"];

        if (Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var parsedBaseUri))
        {
            BaseUri = parsedBaseUri;
            return;
        }

        BaseUri = DefaultBaseUri;
    }
}
