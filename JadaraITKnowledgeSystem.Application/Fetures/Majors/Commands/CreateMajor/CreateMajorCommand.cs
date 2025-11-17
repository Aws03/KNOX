using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Commands.CreateMajor
{
    public sealed record CreateMajorCommand(string Name,int FacultyId) : IRequest<Result<MajorDto>>;
    
}
