namespace Spam;

public class SequentialEntityInsertStrategy(SequentialEntityInsertStrategyOptions options) : IDbContextOperationStrategy
{
    public async Task Execute(AppDbContext dbContext, RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        dbContext.SequentialEntities.Add(new SequentialEntity() { SomeProperty = options.DataCreationStrategy.CreateData() });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}