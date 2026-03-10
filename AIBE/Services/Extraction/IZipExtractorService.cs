namespace AIBE.Services.Extraction;

public interface IZipExtractorService
{
    Task<IReadOnlyList<ExtractedFile>> ExtractAsync(Stream zipStream, CancellationToken cancellationToken = default);
}

public record ExtractedFile(string FileName, string Content);
