using Microsoft.EntityFrameworkCore;

namespace Spam;

public class DbContextSpammerStrategy(DbContextSpammerStrategyOptions options) : ISpammerStrategy
{
    public async Task Prepare(int runnerIndex, Dictionary<object, object> runnerData, CancellationToken cancellationToken)
    {
        if (options.HotConnections)
        {
            await using var dbContext = await options.DbContextFactory.CreateDbContextAsync(cancellationToken);
            await dbContext.SequentialEntities.Take(1).ToListAsync(cancellationToken);
        }
    }

    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        await WithRetry(
            async dbContext => await options.OperationStrategy.Execute(dbContext, context, cancellationToken),
            cancellationToken);
    }

    private async Task WithRetry(
        Func<AppDbContext, Task> func,
        CancellationToken cancellationToken)
    {
        switch (options.DbContextRetryStrategy)
        {
            case DbContextRetryStrategy.FullExpensive:
                await options.DbContextFactory.WithRetry(
                    async dbContext => await func(dbContext),
                    cancellationToken);
                break;
            case DbContextRetryStrategy.None:
            {
                await using var dbContext = await options.DbContextFactory.CreateDbContextAsync(cancellationToken);
                await func(dbContext);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}