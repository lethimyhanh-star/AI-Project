using System.Text;

namespace AIBE.Services.Extraction;

public class ZipExtractorService : IZipExtractorService
{
    private static readonly string[] SupportedExtensions = [".txt", ".md", ".json", ".csv", ".html", ".xml", ".cs", ".js", ".ts", ".py"];
    private const int MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB per file

    public async Task<IReadOnlyList<ExtractedFile>> ExtractAsync(Stream zipStream, CancellationToken cancellationToken = default)
    {
        var results = new List<ExtractedFile>();
        using var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (entry.Length == 0 || entry.FullName.EndsWith('/'))
                continue;

            var ext = Path.GetExtension(entry.Name).ToLowerInvariant();
            if (!SupportedExtensions.Contains(ext))
                continue;

            if (entry.Length > MaxFileSizeBytes)
                continue;

            await using var entryStream = entry.Open();
            using var reader = new StreamReader(entryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var content = await reader.ReadToEndAsync(cancellationToken);
            results.Add(new ExtractedFile(entry.FullName, content));
        }

        return results;
    }
}
