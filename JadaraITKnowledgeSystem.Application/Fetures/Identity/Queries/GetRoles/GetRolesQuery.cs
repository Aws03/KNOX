using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Identity.Queries.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<List<string>>>;
