using JadaraITKnowledgeSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public class UserReactionDto
    {
        public int Id { get; set; }
        public int UserId { get; private set; }
        public int QuizId { get; private set; }
        public bool IsLike { get; private set; }
    }
}
