using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetWriterStatistics
{
    public sealed class GetWriterStatisticsQuery : IRequest<WriterStatisticsDto>
    {
        public int WriterId { get; set; }
        public GetWriterStatisticsQuery(int writerId)
        {
            WriterId = writerId;
        }
    }
}
