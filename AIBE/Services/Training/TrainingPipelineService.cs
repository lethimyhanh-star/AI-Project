using AIBE.Services.Chunking;
using AIBE.Services.Embedding;
using AIBE.Services.Extraction;
using AIBE.Services.VectorStore;
using Microsoft.Extensions.Options;

namespace AIBE.Services.Training;

public class TrainingPipelineService : ITrainingPipelineService
{
    private readonly IZipExtractorService _extractor;
    private readonly IChunkingService _chunking;
    private readonly IEmbeddingService _embedding;
    private readonly IVectorStoreService _vectorStore;
    private readonly string _collectionName;

    public TrainingPipelineService(
        IZipExtractorService extractor,
        IChunkingService chunking,
        IEmbeddingService embedding,
        IVectorStoreService vectorStore,
        IOptions<TrainingSettings> options)
    {
        _extractor = extractor;
        _chunking = chunking;
        _embedding = embedding;
        _vectorStore = vectorStore;
        _collectionName = options.Value.CollectionName ?? "knowledge_base";
    }

    public async Task ExecuteAsync(Stream zipStream, string prompt, CancellationToken cancellationToken = default)
    {
        // Step 1: Unzip và phân tích file
        var extractedFiles = await _extractor.ExtractAsync(zipStream, cancellationToken);
        if (extractedFiles.Count == 0)
            throw new InvalidOperationException("Không tìm thấy file văn bản trong ZIP.");

        // Step 2 & 3: Parse và Chunk dữ liệu
        var allChunks = new List<(string Text, string Source, int ChunkIndex)>();
        foreach (var file in extractedFiles)
        {
            var chunks = _chunking.Chunk(file.Content);
            foreach (var chunk in chunks)
            {
                if (!string.IsNullOrWhiteSpace(chunk.Text))
                    allChunks.Add((chunk.Text, file.FileName, chunk.Index));
            }
        }

        if (allChunks.Count == 0)
            throw new InvalidOperationException("Không có nội dung để xử lý.");

        // Step 4: Generate Embedding
        var texts = allChunks.Select(c => c.Text).ToList();
        var embeddings = await _embedding.GenerateEmbeddingsAsync(texts, cancellationToken);

        // Step 5: Lưu vào Vector Database
        var points = allChunks.Zip(embeddings, (chunk, vec) =>
            new VectorPoint(vec, chunk.Text, chunk.Source, chunk.ChunkIndex)).ToList();

        await _vectorStore.UpsertAsync(_collectionName, prompt, points, cancellationToken);
    }
}

public class TrainingSettings
{
    public const string SectionName = "Training";
    public string CollectionName { get; set; } = "knowledge_base";
}
