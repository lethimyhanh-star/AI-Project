namespace AIBE.Services.VectorStore;

public interface IVectorStoreService
{
    Task UpsertAsync(string collectionName, string prompt, IReadOnlyList<VectorPoint> points, CancellationToken cancellationToken = default);
}

public record VectorPoint(float[] Vector, string Text, string Source, int ChunkIndex);
