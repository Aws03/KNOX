using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Queries.GetMajorsByFacultyId;

public sealed record GetMajorsByFacultyIdQuery(int FacultyId, int PageNumber, int PageSize)
    : IRequest<Result<PaginatedList<MajorDto>>>, IPaginatedRequest;

