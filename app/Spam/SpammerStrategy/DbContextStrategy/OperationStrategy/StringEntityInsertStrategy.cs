namespace Spam;

public class StringEntityInsertStrategy(StringEntityInsertStrategyOptions options) : IDbContextOperationStrategy
{
    public async Task Execute(AppDbContext dbContext, RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        dbContext.RandomKeyEntities.Add(new StringEntity() { Id = options.DataCreationStrategy.CreateData() });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}