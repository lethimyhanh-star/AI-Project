namespace AIBE.Services.Chunking;

public class ChunkingService : IChunkingService
{
    public IReadOnlyList<TextChunk> Chunk(string text, ChunkOptions? options = null)
    {
        options ??= new ChunkOptions();
        var chunkSize = Math.Max(100, options.ChunkSize);
        var overlap = Math.Min(options.OverlapSize, chunkSize / 2);
        var chunks = new List<TextChunk>();
        var index = 0;
        var start = 0;

        while (start < text.Length)
        {
            var length = Math.Min(chunkSize, text.Length - start);
            var chunkText = text[start..(start + length)];
            chunks.Add(new TextChunk(chunkText.Trim(), index++));
            start += chunkSize - overlap;
        }

        return chunks;
    }
}
