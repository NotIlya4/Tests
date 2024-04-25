using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public abstract class DbContextSpammer(
    SpammerOptions spammerOptions,
    DbContextSpammerOptions dbContextSpammerOptions)
    : SpammerBase(spammerOptions)
{
    protected override async Task OnRunnerCreating(
        int runnerIndex,
        Dictionary<object, object> runnerData,
        CancellationToken cancellationToken)
    {
        if (dbContextSpammerOptions.HotConnectionsBeforeSpam)
        {
            await using var dbContext = await dbContextSpammerOptions.DbContextFactory.CreateDbContextAsync(cancellationToken);
            await dbContext.Entities.Take(1).ToListAsync(cancellationToken);
        }
    }

    protected override async Task Execute(
        RunnerExecutionContext context, 
        CancellationToken cancellationToken)
    {
        await dbContextSpammerOptions.DbContextFactory.WithRetryAsync(dbContext => ExecuteCore(dbContext, context, cancellationToken),
            cancellationToken);
    }

    protected abstract Task ExecuteCore(
        AppDbContext dbContext, 
        RunnerExecutionContext context,
        CancellationToken cancellationToken);
}