using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizById;

public sealed record GetQuizByIdQuery(int QuizId)
: IRequest<Result<QuizDto>>;
