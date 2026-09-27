using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface ITextExtractionService
{
    Task<Result<string>> ExtractTextAsync(Stream fileStream, string extension, CancellationToken cancellationToken = default);    
    Task<List<TextChunk>> ChunkTextIntelligentlyAsync(string text, ChunkingOptions options, CancellationToken cancellationToken = default);
    
    bool SupportsFileType(string extension);
}

public sealed class TextChunk
{
    public string Text { get; set; } = string.Empty;
    public string? DetectedTopic { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
}

public sealed class ChunkingOptions
{
    public int MaxCharsPerChunk { get; set; } = 4000;
    public int MinCharsPerChunk { get; set; } = 1000;
    public bool SplitBySection { get; set; } = true;
    public int OverlapPercentage { get; set; } = 10;
}
