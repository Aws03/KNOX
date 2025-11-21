using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using System;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;

public record CreateQuestionDto
{
    public string Text { get; init; } = String.Empty;
    public int QuizId { get; init; }
    public string? ImageUrl { get; set; }
    public QuestionType Type { get; init; } // 1: Single Choice, 2: Multiple Choice, 3: True/False, 4: Short Answer
    public List<CreateChoiceDto> Choices { get; init; } = new();
}
