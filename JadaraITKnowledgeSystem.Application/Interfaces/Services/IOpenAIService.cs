using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface IOpenAIService
{
    Task<Result<GeneratedQuizDto>> GenerateQuizFromTextAsync(
        GenerateQuizRequest request, 
        CancellationToken cancellationToken = default);
    
    Task<Result<string>> GenerateTextAsync(
        string prompt, 
        CancellationToken cancellationToken = default);
    
    Task<Result<List<string>>> ExtractTopicsAsync(
        string text, 
        int maxTopics = 5,
        CancellationToken cancellationToken = default);
}

public sealed class GenerateQuizRequest
{
    public string Text { get; set; }
    public int QuestionCount { get; set; } = 8;
    public string Difficulty { get; set; } = "Medium";
    public int ChunkIndex { get; set; } = 0;
    public int TotalChunks { get; set; } = 1;
}

public sealed class GeneratedQuizDto
{
    public string Topic { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> SuggestedTags { get; set; }
    public List<CreateQuestionDto> Questions { get; set; }
}
