using JadaraITKnowledgeSystem.Application.Common.Behaviours;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Infrastructure.Services.BackgroundJobs;
using JadaraITKnowledgeSystem.IntegrationTests.TestSupport;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace JadaraITKnowledgeSystem.IntegrationTests.Application;

/// <summary>Transaction and post-commit dispatch semantics on SQL Server with the production retry strategy.</summary>
[Collection(InfrastructureCollection.Name)]
public class TransactionBehaviorTests(InfrastructureFixture database)
{
    public sealed record SampleCommand(string Name) : IRequest<Result<int>>;
    public sealed record LongRunningCommand : IRequest<Result<int>>, INonTransactionalCommand;
    public sealed record SampleQuery : IRequest<Result<int>>;

    private static TransactionBehavior<TRequest, Result<int>> Behavior<TRequest>(IApplicationDbContext context)
        where TRequest : IRequest<Result<int>> =>
        new(context, NullLogger<TransactionBehavior<TRequest, Result<int>>>.Instance);

    [Fact]
    public async Task Command_RunsInsideATransaction()
    {
        await using var context = database.CreateContext();
        var hadTransaction = false;

        await Behavior<SampleCommand>(context).Handle(new SampleCommand("x"), _ =>
        {
            hadTransaction = context.Database.CurrentTransaction is not null;
            return Task.FromResult<Result<int>>(1);
        }, default);

        Assert.True(hadTransaction);
        Assert.Null(context.Database.CurrentTransaction);
    }

    [Fact]
    public async Task NestedCommand_JoinsTheOuterTransaction()
    {
        await using var context = database.CreateContext();

        var result = await Behavior<SampleCommand>(context).Handle(new SampleCommand("outer"), _ =>
            Behavior<SampleCommand>(context).Handle(new SampleCommand("inner"), _ => Task.FromResult<Result<int>>(5), default), default);

        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task ExceptionInHandler_RollsBackEverything()
    {
        await using var context = database.CreateContext();
        var (_, _, major) = await context.SeedHierarchyAsync();
        var email = TestData.UniqueEmail();

        await Assert.ThrowsAsync<InvalidOperationException>(() => Behavior<SampleCommand>(context).Handle(new SampleCommand("x"), async _ =>
        {
            await context.SeedUserAsync(major.Id, email);
            throw new InvalidOperationException("boom");
        }, default));

        await using var verify = database.CreateContext();
        Assert.False(await verify.Users.AnyAsync(u => u.Email.Address == email.ToUpperInvariant()));
    }

    [Fact]
    public async Task QueriesAndOptedOutCommands_RunWithoutATransaction()
    {
        await using var context = database.CreateContext();
        var transactions = new List<bool>();

        await Behavior<SampleQuery>(context).Handle(new SampleQuery(), _ =>
        {
            transactions.Add(context.Database.CurrentTransaction is not null);
            return Task.FromResult<Result<int>>(1);
        }, default);
        await Behavior<LongRunningCommand>(context).Handle(new LongRunningCommand(), _ =>
        {
            transactions.Add(context.Database.CurrentTransaction is not null);
            return Task.FromResult<Result<int>>(1);
        }, default);

        Assert.Equal(new[] { false, false }, transactions);
    }

    [Fact]
    public async Task PostCommitWork_IsQueuedOnlyByTheOutermostRequest()
    {
        await using var context = database.CreateContext();
        var dispatcher = new PostCommitDispatcher();
        var queue = Substitute.For<IBackgroundJobQueue>();
        var dispatch = new DispatchPostCommitJobsBehavior<SampleCommand, Result<int>>(
            dispatcher, queue, context, NullLogger<DispatchPostCommitJobsBehavior<SampleCommand, Result<int>>>.Instance);

        // Outer command: dispatch wraps the transaction; the inner command stages work while the
        // outer transaction is still open, so nothing may be queued until the outer one commits.
        await dispatch.Handle(new SampleCommand("outer"), _ =>
            Behavior<SampleCommand>(context).Handle(new SampleCommand("outer"), async _ =>
            {
                await dispatch.Handle(new SampleCommand("inner"), _ =>
                {
                    dispatcher.Enqueue((_, _) => Task.CompletedTask);
                    return Task.FromResult<Result<int>>(1);
                }, default);

                queue.DidNotReceiveWithAnyArgs().QueueBackgroundWorkItem(default!);
                return 1;
            }, default), default);

        queue.ReceivedWithAnyArgs(1).QueueBackgroundWorkItem(default!);
    }
}
