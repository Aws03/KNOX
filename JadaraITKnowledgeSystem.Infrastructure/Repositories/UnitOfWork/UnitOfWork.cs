using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction _transaction;

        // Private fields for lazy initialization
        private ICourseRepository _courseRepository;
        private IChoiceRepository _choiceRepository;
        private ICourseMaterialRepository _courseMaterialRepository;
        private ICourseRequirementMappingRepository _courseRequirementMappingRepository;
        private IFacultyRepository _facultyRepository;
        private IMajorRepository _majorRepository;
        private IQuestionRepository _questionRepository;
        private IQuizRepository _quizRepository;
        private IQuizAttemptRepository _quizAttemptRepository;
        private IUniversityRepository _universityRepository;
        private IUserRepository _userRepository;
        private IUserReactionRepository _userReactionRepository;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Lazy-loaded repository properties
        public ICourseRepository Courses
        {
            get
            {
                _courseRepository ??= new CourseRepository(_dbContext);
                return _courseRepository;
            }
        }

        public IChoiceRepository Choices
        {
            get
            {
                _choiceRepository ??= new ChoiceRepository(_dbContext);
                return _choiceRepository;
            }
        }

        public ICourseMaterialRepository CourseMaterials
        {
            get
            {
                _courseMaterialRepository ??= new CourseMaterialRepository(_dbContext);
                return _courseMaterialRepository;
            }
        }

        public ICourseRequirementMappingRepository Enrollments
        {
            get
            {
                _courseRequirementMappingRepository ??= new CourseRequirementMappingRepository(_dbContext);
                return _courseRequirementMappingRepository;
            }
        }

        public IFacultyRepository Faculties
        {
            get
            {
                _facultyRepository ??= new FacultyRepository(_dbContext);
                return _facultyRepository;
            }
        }

        public IMajorRepository Majors
        {
            get
            {
                _majorRepository ??= new MajorRepository(_dbContext);
                return _majorRepository;
            }
        }

        public IQuestionRepository Questions
        {
            get
            {
                _questionRepository ??= new QuestionRepository(_dbContext);
                return _questionRepository;
            }
        }

        public IQuizAttemptRepository QuizAttempts
        {
            get
            {
                _quizAttemptRepository ??= new QuizAttemptRepository(_dbContext);
                return _quizAttemptRepository;
            }
        }

        public IQuizRepository Quizzes
        {
            get
            {
                _quizRepository ??= new QuizRepository(_dbContext);
                return _quizRepository;
            }
        }

        public IUniversityRepository Universities
        {
            get
            {
                _universityRepository ??= new UniversityRepository(_dbContext);
                return _universityRepository;
            }
        }

        public IUserReactionRepository UserReactions
        {
            get
            {
                _userReactionRepository ??= new UserReactionRepository(_dbContext);
                return _userReactionRepository;
            }
        }

        public IUserRepository Users
        {
            get
            {
                _userRepository ??= new UserRepository(_dbContext);
                return _userRepository;
            }
        }

        public ICourseRequirementMappingRepository CourseRequirementMapping => throw new NotImplementedException();

        // Transaction Management
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose();
        }

       
    }
}
