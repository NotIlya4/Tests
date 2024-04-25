using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public class SmartDbContextSpammerBuilder(
    IDbContextFactory<AppDbContext> dbContextFactory,
    IEnumerable<IDbContextSpammerStrategy> dbContextSpammerStrategies,
    SpammerBuilderBaseDependencies dependencies)
    : SpammerBuilderBase<SmartDbContextSpammerBuilder, SmartDbContextSpammer>(dependencies)
{
    private SpammerStrategyType _type;

    public SmartDbContextSpammerBuilder WithStrategyType(SpammerStrategyType type)
    {
        _type = type;
        return this;
    }

    protected override SmartDbContextSpammer Create(SpammerOptions spammerOptions)
    {
        return new SmartDbContextSpammer(
            spammerOptions,
            new DbContextSpammerOptions()
            {
                DbContextFactory = dbContextFactory,
                HotConnectionsBeforeSpam = true
            },
            new SmartDbContextSpammerOptions()
            {
                SpammerStrategyType = _type
            },
            dbContextSpammerStrategies);
    }
}