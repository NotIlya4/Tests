namespace Spam;

public class ParallelForAsyncParallelEngine : ISpammerParallelEngine
{
    public async Task ParallelRun(int parallels, Func<int, ValueTask> func, CancellationToken cancellationToken)
    {
        await Parallel.ForAsync(
            0,
            parallels,
            new ParallelOptions() { MaxDegreeOfParallelism = parallels, CancellationToken = cancellationToken },
            (i, _) => func(i));
    }
}