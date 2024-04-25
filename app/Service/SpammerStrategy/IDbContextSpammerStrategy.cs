using Spammer;

namespace Service;

public interface IDbContextSpammerStrategy
{
    SpammerStrategyType Type { get; }

    Task Execute(AppDbContext dbContext, RunnerExecutionContext context, CancellationToken cancellationToken);
}