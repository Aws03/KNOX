
namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;

public sealed record QuizAttemptDto
{
    public int Id { get; init; }
    public int QuizId { get; init; }
    public int UserId { get; init; }
    public decimal Score { get; init; }
    public DateTime AttemptDate { get; init; }
}
