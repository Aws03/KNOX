using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface IFeatureFlagService
{
    bool IsQuizGenerationEnabled();

    Task<bool> ToggleQuizGenerationAsync(bool enabled, CancellationToken cancellationToken = default);

    Task<FeatureFlagsDto> GetAllFlagsAsync(CancellationToken cancellationToken = default);
}

public sealed class FeatureFlagsDto
{
    public bool QuizGenerationEnabled { get; set; }
    public int MaxConcurrentJobs { get; set; }
    public int DefaultQuestionsPerQuiz { get; set; }
}
