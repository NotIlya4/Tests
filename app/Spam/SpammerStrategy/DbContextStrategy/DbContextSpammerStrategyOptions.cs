using Microsoft.EntityFrameworkCore;

namespace Spam;

public class DbContextSpammerStrategyOptions
{
    public required bool HotConnections { get; set; }
    public required DbContextRetryStrategy DbContextRetryStrategy { get; set; }
    public required IDbContextOperationStrategy OperationStrategy { get; set; }
    public required IDbContextFactory<AppDbContext> DbContextFactory { get; set; }
}