using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Features.Identity.Queries.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<List<string>>>;
