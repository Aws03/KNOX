using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;

namespace JadaraITKnowledgeSystem.API.Contracts;

/// <summary>The author is always the authenticated caller, so there is no writer id here.</summary>
public sealed record CreateQuizRequest(
    string Title,
    int CourseId,
    string? Description,
    List<CreateQuestionDto> Questions,
    List<string>? Tags);
