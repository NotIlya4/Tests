namespace Spam;

public interface ISpammerParallelEngine
{
    Task ParallelRun(int parallels, Func<int, ValueTask> func, CancellationToken cancellationToken);
}