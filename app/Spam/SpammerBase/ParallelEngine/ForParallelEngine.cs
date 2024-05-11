namespace Spam;

public class ForParallelEngine : ISpammerParallelEngine
{
    public async Task ParallelRun(int parallels, Func<int, ValueTask> func, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>(parallels);
        
        for (int i = 0; i < parallels; i++)
        {
            var i1 = i;
            tasks.Add(Task.Run(async () => await func(i1), cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
}