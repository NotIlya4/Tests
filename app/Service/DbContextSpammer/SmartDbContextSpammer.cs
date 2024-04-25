using Spammer;

namespace Service;

public class SmartDbContextSpammer(
    SpammerOptions spammerOptions,
    DbContextSpammerOptions dbContextSpammerOptions,
    SmartDbContextSpammerOptions options,
    IEnumerable<IDbContextSpammerStrategy> dbContextSpammerStrategies)
    : DbContextSpammer(spammerOptions, dbContextSpammerOptions)
{
    protected override async Task ExecuteCore(AppDbContext dbContext, RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        var strategy = dbContextSpammerStrategies.Single(x => x.Type == options.SpammerStrategyType);
        await strategy.Execute(dbContext, context, cancellationToken);
    }
}