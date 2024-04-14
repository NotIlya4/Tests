namespace Service;

public class SqlServerSpammerBuilderFactory(SqlServerSpammerDependencies spammerDependencies, SpammerBuilderBaseDependencies spammerBuilderBaseDependencies)
{
    public SqlServerSpammerBuilder Create()
    {
        return new SqlServerSpammerBuilder(spammerDependencies, spammerBuilderBaseDependencies);
    }
}