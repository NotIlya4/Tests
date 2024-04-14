namespace Service;

public class SqlServerEntityCreationStrategyProvider
{
    public ISqlServerEntityCreationStrategy Create(SqlServerSpammerEntityType entityType)
    {
        return entityType switch
        {
            SqlServerSpammerEntityType.Entity => new EntitySqlServerEntityCreationStrategy(),
            SqlServerSpammerEntityType.GuidEntity => new GuidEntitySqlServerEntityCreationStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null)
        };
    }
}