using System.Collections.Generic;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobsByMaterial;

public sealed record GetQuizGenerationJobsByMaterialQuery(int MaterialId) 
    : IRequest<Result<List<QuizGenerationJobDto>>>;
