using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface ITextExtractionService
{
    Task<Result<string>> ExtractTextAsync(Stream fileStream, string extension, CancellationToken cancellationToken = default);
    
    Task<Result<ExtractedTextDto>> ExtractTextWithMetadataAsync(Stream fileStream, string extension, CancellationToken cancellationToken = default);
    
    Task<List<TextChunk>> ChunkTextIntelligentlyAsync(string text, ChunkingOptions options, CancellationToken cancellationToken = default);
    
    bool SupportsFileType(string extension);
}

public sealed class ExtractedTextDto
{
    public string Text { get; set; }
    public string? DetectedLanguage { get; set; }
    public int CharacterCount { get; set; }
    public List<string>? DetectedTopics { get; set; }
}

public sealed class TextChunk
{
    public string Text { get; set; }
    public string? DetectedTopic { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
}

public sealed class ChunkingOptions
{
    public int MaxCharsPerChunk { get; set; } = 4000;
    public int MinCharsPerChunk { get; set; } = 1000;
    public int QuestionsPerChunk { get; set; } = 8;
    public bool SplitBySection { get; set; } = true;
    public int OverlapPercentage { get; set; } = 10;
}
