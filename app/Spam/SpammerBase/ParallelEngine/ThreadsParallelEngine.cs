namespace Spam;

public class ThreadsParallelEngine : ISpammerParallelEngine
{
    public Task ParallelRun(int parallels, Func<int, ValueTask> func, CancellationToken cancellationToken)
    {
        Parallel.For(
            0,
            parallels,
            new ParallelOptions() { MaxDegreeOfParallelism = parallels, CancellationToken = cancellationToken },
            i => func(i).GetAwaiter().GetResult());
        return Task.CompletedTask;
    }
}