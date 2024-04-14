using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public class SqlServerSpammerDependencies(
    IDbContextFactory<AppDbContext> dbContextFactory,
    SqlServerEntityCreationStrategyProvider strategyProvider)
{
    public IDbContextFactory<AppDbContext> DbContextFactory { get; } = dbContextFactory;
    public SqlServerEntityCreationStrategyProvider StrategyProvider { get; } = strategyProvider;
}