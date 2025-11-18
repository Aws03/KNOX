using JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Commands.CreateUniversity
{
    public sealed record CreateUniversityCommand(string Name) : IRequest<Result<UniversityDto>>;
    
}
