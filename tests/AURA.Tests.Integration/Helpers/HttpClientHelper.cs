namespace AURA.Tests.Integration.Helpers;

public static class HttpClientHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<ApiResponse<T>> ReadApiResponseAsync<T>(HttpResponseMessage response)
    {
        var model = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions);
        model.Should().NotBeNull();
        return model!;
    }

    public static async Task<JsonDocument> ReadJsonDocumentAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        json.Should().NotBeNullOrWhiteSpace();
        return JsonDocument.Parse(json);
    }
}
