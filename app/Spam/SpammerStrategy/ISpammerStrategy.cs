namespace Spam;

public interface ISpammerStrategy
{
    Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken);
}