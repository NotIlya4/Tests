using UUIDNext;

namespace Spam;

public class GuidDataCreationStrategy : ISimpleDataCreationStrategy<Guid>, ISimpleDataCreationStrategy<string>
{
    Guid ISimpleDataCreationStrategy<Guid>.CreateData()
    {
        return CreateCore();
    }

    string ISimpleDataCreationStrategy<string>.CreateData()
    {
        return CreateCore().ToString();
    }

    private Guid CreateCore()
    {
        return Guid.NewGuid();
    }
}

public class SeqGuidDataCreationStrategy : ISimpleDataCreationStrategy<Guid>, ISimpleDataCreationStrategy<string>
{
    Guid ISimpleDataCreationStrategy<Guid>.CreateData()
    {
        return CreateCore();
    }

    string ISimpleDataCreationStrategy<string>.CreateData()
    {
        return CreateCore().ToString();
    }

    private Guid CreateCore()
    {
        return Uuid.NewDatabaseFriendly(Database.PostgreSql);
    }
}