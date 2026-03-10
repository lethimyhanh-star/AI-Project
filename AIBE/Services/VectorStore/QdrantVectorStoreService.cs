using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AIBE.Services.VectorStore;

public class QdrantVectorStoreService : IVectorStoreService
{
    private readonly QdrantClient _client;
    private readonly QdrantSettings _settings;

    public QdrantVectorStoreService(IOptions<QdrantSettings> options)
    {
        _settings = options.Value;
        _client = new QdrantClient(_settings.Host, _settings.Port, _settings.UseTls);
    }

    public async Task UpsertAsync(string collectionName, string prompt, IReadOnlyList<VectorPoint> points, CancellationToken cancellationToken = default)
    {
        if (points.Count == 0) return;

        var vectorSize = (ulong)points[0].Vector.Length;
        var collectionExists = await _client.CollectionExistsAsync(collectionName, cancellationToken);

        if (!collectionExists)
        {
            await _client.CreateCollectionAsync(
                collectionName,
                new VectorParams { Size = vectorSize, Distance = Distance.Cosine },
                cancellationToken: cancellationToken);
        }

        var pointsData = new List<PointStruct>();
        var baseId = (ulong)DateTime.UtcNow.Ticks;
        for (var i = 0; i < points.Count; i++)
        {
            var p = points[i];
            pointsData.Add(new PointStruct
            {
                Id = new PointId { Num = baseId + (ulong)i },
                Vectors = p.Vector,
                Payload =
                {
                    ["text"] = p.Text,
                    ["source"] = p.Source,
                    ["chunk_index"] = p.ChunkIndex,
                    ["prompt"] = prompt
                }
            });
        }

        await _client.UpsertAsync(collectionName, pointsData, cancellationToken: cancellationToken);
    }
}

public class QdrantSettings
{
    public const string SectionName = "Qdrant";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6334;
    public bool UseTls { get; set; }
}
