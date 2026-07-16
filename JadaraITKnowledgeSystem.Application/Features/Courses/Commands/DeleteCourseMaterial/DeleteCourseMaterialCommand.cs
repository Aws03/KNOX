using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteCourseMaterial;

public sealed record DeleteCourseMaterialCommand(int MaterialId) : IRequest<Result<Success>>;
