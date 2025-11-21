using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;

public sealed record UserReactionDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int QuizId { get; init; }
    public ReactionType ReactionType { get; init; }
}
