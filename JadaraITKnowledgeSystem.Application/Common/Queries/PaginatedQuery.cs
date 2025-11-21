using MediatR;

namespace JadaraITKnowledgeSystem.Application.Common.Queries;

public abstract record PaginatedQuery<TResponse>(
int PageNumber = 1,
int PageSize = 10
) : IRequest<TResponse>;
