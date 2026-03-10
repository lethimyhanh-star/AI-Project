namespace AIBE.Services.Chunking;

public interface IChunkingService
{
    IReadOnlyList<TextChunk> Chunk(string text, ChunkOptions? options = null);
}

public record TextChunk(string Text, int Index);

public class ChunkOptions
{
    public int ChunkSize { get; set; } = 1000;
    public int OverlapSize { get; set; } = 200;
}
