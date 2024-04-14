using Spammer;

namespace Service;

public class EntitySqlServerEntityCreationStrategy : SqlServerEntityCreationStrategyBase
{
    protected override async Task CreateCore(AppDbContext dbContext, RunnerExecutionContext executionContext)
    {
        dbContext.Entities.Add(new Entity { SomeProperty = Guid.NewGuid().ToString() });
        await dbContext.SaveChangesAsync();
    }
}