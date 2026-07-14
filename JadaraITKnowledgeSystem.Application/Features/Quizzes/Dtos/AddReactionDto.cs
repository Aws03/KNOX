using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
public sealed class AddReactionDto
{
    public int UserId { get; init; }
    public ReactionType ReactionType { get; init; }
}
