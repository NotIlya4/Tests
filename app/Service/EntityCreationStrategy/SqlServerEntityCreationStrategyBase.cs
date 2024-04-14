using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public abstract class SqlServerEntityCreationStrategyBase : ISqlServerEntityCreationStrategy
{
    public async Task Create(RunnerExecutionContext context)
    {
        var dbContextFactory = (IDbContextFactory<AppDbContext>)context.Data["dbContextFactory"];
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var localDbContext = await dbContextFactory.CreateDbContextAsync();
            await CreateCore(localDbContext, context);
        });
    }

    protected abstract Task CreateCore(AppDbContext dbContext, RunnerExecutionContext executionContext);
}