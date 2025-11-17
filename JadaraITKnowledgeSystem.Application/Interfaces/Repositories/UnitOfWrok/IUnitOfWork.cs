using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok
{
    public interface IUnitOfWork : IDisposable
    {
        IChoiceRepository Choices { get; }
        ICourseMaterialRepository CourseMaterials { get; }
        ICourseRepository Courses { get; }
        ICourseRequirementMappingRepository CourseRequirementMapping { get; }
        IFacultyRepository Faculties { get; }
        IMajorRepository Majors { get; }
        IQuestionRepository Questions { get; }
        IQuizAttemptRepository QuizAttempts { get; }
        IQuizRepository Quizzes { get; }
        IUniversityRepository Universities { get; }
        IUserReactionRepository UserReactions { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
