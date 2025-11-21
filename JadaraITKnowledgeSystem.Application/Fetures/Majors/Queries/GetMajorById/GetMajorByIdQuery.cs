using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Queries.GetMajorById;

public sealed record GetMajorByIdQuery(int MajorId) : IRequest<Result<MajorDto>>;

