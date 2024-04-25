using Spammer;

namespace Service;

public class EntitySpammerStrategy : IDbContextSpammerStrategy
{
    public SpammerStrategyType Type => SpammerStrategyType.Entity;
    
    public async Task Execute(
        AppDbContext dbContext,
        RunnerExecutionContext context,
        CancellationToken cancellationToken)
    {
        dbContext.Add(new Entity() { SomeProperty = Guid.NewGuid().ToString() });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}