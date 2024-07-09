namespace Spam;

public class SequentialEntityInsertStrategy(SequentialEntityInsertStrategyOptions options) : IDbContextOperationStrategy
{
    public async Task Execute(AppDbContext dbContext, RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        dbContext.SequentialKeyEntities.Add(new SequentialEntity() { SomeProperty = options.DataCreationStrategy.CreateData() });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}