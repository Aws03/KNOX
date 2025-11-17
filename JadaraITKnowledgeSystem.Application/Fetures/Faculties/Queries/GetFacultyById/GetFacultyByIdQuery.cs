using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultyById
{
    public sealed record GetFacultyByIdQuery(int facultyId) : IRequest<Result<FacultyDto>>;


}
