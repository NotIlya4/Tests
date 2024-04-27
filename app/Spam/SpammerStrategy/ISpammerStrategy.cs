namespace Spam;

public interface ISpammerStrategy
{
    Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken);

    Task Prepare(int runnerIndex,
        Dictionary<object, object> runnerData,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}