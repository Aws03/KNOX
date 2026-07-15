using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourseMaterial;

public sealed record CreateCourseMaterialCommand(
    string Title,
    string ContemtUrl,
    int CourseId,
    int? FolderId,
    string? Description,
    IEnumerable<string>? Tags,
    bool GenerateQuiz = false,
    QuizGenerationOptionsDto? QuizOptions = null
) : IRequest<Result<CourseMaterialDto>>;
