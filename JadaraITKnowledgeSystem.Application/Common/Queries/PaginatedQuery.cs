using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Common.Queries
{
    public abstract record PaginatedQuery<TResponse>(
    int PageNumber = 1,
    int PageSize = 10
    ) : IRequest<TResponse>;
}
