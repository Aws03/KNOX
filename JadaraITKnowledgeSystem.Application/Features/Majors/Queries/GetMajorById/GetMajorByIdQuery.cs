using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Queries.GetMajorById;

public sealed record GetMajorByIdQuery(int MajorId) : IRequest<Result<MajorDto>>;

