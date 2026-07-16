using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Commands.UpdateMajor;

public sealed record UpdateMajorCommand(int Id, string Name, int FacultyId) : IRequest<Result<MajorDto>>;
