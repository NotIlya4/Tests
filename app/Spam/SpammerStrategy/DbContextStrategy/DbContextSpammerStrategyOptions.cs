using Microsoft.EntityFrameworkCore;

namespace Spam;

public class DbContextSpammerStrategyOptions
{
    public required DbContextRetryStrategyType DbContextRetryStrategyType { get; set; }
    public required IDbContextOperationStrategy OperationStrategy { get; set; }
    public required IDbContextFactory<AppDbContext> DbContextFactory { get; set; }
}