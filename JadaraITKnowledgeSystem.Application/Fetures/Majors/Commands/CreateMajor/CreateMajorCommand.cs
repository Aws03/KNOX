using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Commands.CreateMajor;

public sealed record CreateMajorCommand(string Name,int FacultyId) : IRequest<Result<MajorDto>>;

