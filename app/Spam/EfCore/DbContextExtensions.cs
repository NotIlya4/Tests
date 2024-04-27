using Microsoft.EntityFrameworkCore;

namespace Spam;

public static class DbContextExtensions
{
    public static async Task<TResponse> WithRetry<TDbContext, TResponse>(
        this IDbContextFactory<TDbContext> dbContextFactory,
        Func<TDbContext, Task<TResponse>> func,
        CancellationToken cancellationToken) where TDbContext : DbContext
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var strategy = dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async (ct) =>
        {
            await using var localDbContext = await dbContextFactory.CreateDbContextAsync(ct);
            return await func(localDbContext);
        }, cancellationToken);
    }

    public static async Task WithRetry<TDbContext>(
        this IDbContextFactory<TDbContext> dbContextFactory,
        Func<TDbContext, Task> func,
        CancellationToken cancellationToken) where TDbContext : DbContext
    {
        await dbContextFactory.WithRetry(async (d) =>
        {
            await func(d);
            return true;
        }, cancellationToken);
    }
}