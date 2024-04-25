using Spammer;

namespace Service;

public class GuidEntitySpammerStrategy : IDbContextSpammerStrategy
{
    public SpammerStrategyType Type => SpammerStrategyType.GuidEntity;
    
    public async Task Execute(
        AppDbContext dbContext,
        RunnerExecutionContext context,
        CancellationToken cancellationToken)
    {
        dbContext.Add(new GuidEntity() { Id = Guid.NewGuid().ToString() });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}