namespace JadaraITKnowledgeSystem.Domain.Quizzes.Enums;

public enum QuizGenerationStatus
{
    Pending = 0,
    Extracting = 1,
    GeneratingQuizzes = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}
