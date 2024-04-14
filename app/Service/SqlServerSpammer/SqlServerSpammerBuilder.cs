using Spammer;

namespace Service;

public class SqlServerSpammerBuilder(SqlServerSpammerDependencies spammerDependencies, SpammerBuilderBaseDependencies spammerBuilderBaseDependencies)
    : SpammerBuilderBase<SqlServerSpammerBuilder, SqlServerSpammer>(spammerBuilderBaseDependencies)
{
    private SqlServerSpammerEntityType? _entityType;

    public SqlServerSpammerBuilder WithEntityType(SqlServerSpammerEntityType entityType)
    {
        _entityType = entityType;
        return this;
    }
    
    protected override SqlServerSpammer Create(SpammerOptions spammerOptions)
    {
        ArgumentNullException.ThrowIfNull(_entityType);

        return new SqlServerSpammer(spammerDependencies, spammerOptions, _entityType.Value);
    }
}