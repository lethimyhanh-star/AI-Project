using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace AIBE.Services.Embedding;

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public OpenAIEmbeddingService(HttpClient httpClient, IOptions<OpenAISettings> options)
    {
        _httpClient = httpClient;
        var settings = options.Value;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.ApiKey);
        _model = settings.EmbeddingModel ?? "text-embedding-3-small";
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var result = await GenerateEmbeddingsAsync([text], cancellationToken);
        return result[0];
    }

    public async Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        var list = texts.ToList();
        if (list.Count == 0) return Array.Empty<float[]>();

        var request = new { input = list, model = _model };
        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/embeddings", request, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(_jsonOptions, cancellationToken);
        if (json?.Data == null)
            throw new InvalidOperationException("Invalid embedding response");

        return json.Data.OrderBy(d => d.Index).Select(d => d.Embedding.ToArray()).ToArray();
    }

    private class EmbeddingResponse
    {
        public List<EmbeddingData>? Data { get; set; }
    }

    private class EmbeddingData
    {
        public int Index { get; set; }
        public float[] Embedding { get; set; } = [];
    }
}

public class OpenAISettings
{
    public const string SectionName = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}
