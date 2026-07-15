using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.GenerateQuizFromMaterial;

public sealed record GenerateQuizFromMaterialCommand(
    int MaterialId,
    int WriterId,
    QuizGenerationOptionsDto Options
) : IRequest<Result<QuizGenerationJobDto>>;
