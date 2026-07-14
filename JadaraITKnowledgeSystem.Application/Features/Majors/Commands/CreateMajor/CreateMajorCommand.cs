using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Commands.CreateMajor;

public sealed record CreateMajorCommand(string Name,int FacultyId) : IRequest<Result<MajorDto>>;

