using Spammer;

namespace Service;

public class GuidEntitySqlServerEntityCreationStrategy : SqlServerEntityCreationStrategyBase
{
    protected override async Task CreateCore(AppDbContext dbContext, RunnerExecutionContext executionContext)
    {
        dbContext.GuidEntities.Add(new GuidEntity { Id = Guid.NewGuid().ToString() });
        await dbContext.SaveChangesAsync();
    }
}