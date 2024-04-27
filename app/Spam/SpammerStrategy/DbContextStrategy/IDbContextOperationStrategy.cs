namespace Spam;

public interface IDbContextOperationStrategy
{
    Task Execute(AppDbContext dbContext, RunnerExecutionContext context, CancellationToken cancellationToken);
}