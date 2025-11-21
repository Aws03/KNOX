using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Common.Behaviours;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IApplicationDbContext context,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
TRequest request,
RequestHandlerDelegate<TResponse> next,
CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (!requestName.EndsWith("Command"))
        {
            return await next();
        }

        _logger.LogInformation("Beginning transaction for {RequestName}", requestName);

        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            state: (context: _context, next, requestName),
            operation: async (dbContext, state, ct) =>
            {
                await using var transaction = await state.context.Database.BeginTransactionAsync(ct);

                try
                {
                    var response = await state.next();

                    await transaction.CommitAsync(ct);

                    _logger.LogInformation("Committed transaction for {RequestName}", state.requestName);

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Rolling back transaction for {RequestName}", state.requestName);
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            },
            verifySucceeded: null, 
            cancellationToken: cancellationToken
        );

    }

}
