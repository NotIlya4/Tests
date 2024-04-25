using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public class SmartDbContextSpammerBuilderFactory(IEnumerable<IDbContextSpammerStrategy> dbContextSpammerStrategies, SpammerBuilderBaseDependencies spammerBuilderBaseDependencies)
{
    public SmartDbContextSpammerBuilder Create(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        return new SmartDbContextSpammerBuilder(
            dbContextFactory,
            dbContextSpammerStrategies,
            spammerBuilderBaseDependencies);
    }
}