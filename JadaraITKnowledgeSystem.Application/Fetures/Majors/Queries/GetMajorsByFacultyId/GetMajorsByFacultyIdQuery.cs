using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;


namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Queries.GetMajorByFacultyId;

public sealed record GetMajorsByFacultyIdQuery(int FacultyId
    ,int PageNumber
    ,int PageSize) : IRequest<Result<PaginatedList<MajorDto>>>;

