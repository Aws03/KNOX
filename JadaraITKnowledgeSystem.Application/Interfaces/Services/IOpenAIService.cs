using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface IOpenAIService
{
    Task<Result<GeneratedQuizDto>> GenerateQuizFromTextAsync(
        GenerateQuizRequest request, 
        CancellationToken cancellationToken = default);
}

public sealed class GenerateQuizRequest
{
    public string Text { get; set; } = string.Empty;
    public int QuestionCount { get; set; } = 8;
    public string Difficulty { get; set; } = "Medium";
    public int ChunkIndex { get; set; } = 0;
    public int TotalChunks { get; set; } = 1;
}

public sealed class GeneratedQuizDto
{
    public string Topic { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> SuggestedTags { get; set; } = [];
    public List<CreateQuestionDto> Questions { get; set; } = [];
}
