using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos
{
    public sealed class AddReactionDto
    {
        public int UserId { get; init; }
        public ReactionType ReactionType { get; init; }
    }
}
