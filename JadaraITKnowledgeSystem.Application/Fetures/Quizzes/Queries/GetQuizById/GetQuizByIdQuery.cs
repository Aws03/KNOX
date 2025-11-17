using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Queries.GetQuizById
{
    public sealed record GetQuizByIdQuery(int QuizId)
    : IRequest<Result<QuizDto>>;

}
