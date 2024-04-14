using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public class SqlServerSpammer(
    SqlServerSpammerDependencies sqlServerSpammerDependencies,
    SpammerOptions spammerOptions,
    SqlServerSpammerEntityType entityType) : SpammerBase(spammerOptions)
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory = sqlServerSpammerDependencies.DbContextFactory;
    private readonly SqlServerEntityCreationStrategyProvider _strategyProvider = sqlServerSpammerDependencies.StrategyProvider;
    
    protected override async Task OnRunnerCreating(
        int runnerIndex,
        Dictionary<object, object> runnerData)
    {
        runnerData["dbContextFactory"] = _dbContextFactory;

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Entities.Take(1).ToListAsync();
    }

    protected override async Task ExecuteAsync(RunnerExecutionContext context)
    {
        await _strategyProvider.Create(entityType).Create(context);
    }
}