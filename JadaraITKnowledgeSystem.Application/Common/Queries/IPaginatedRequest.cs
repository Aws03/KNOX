namespace JadaraITKnowledgeSystem.Application.Common.Queries;

public interface IPaginatedRequest
{
    int PageNumber { get; }
    int PageSize { get; }
}
