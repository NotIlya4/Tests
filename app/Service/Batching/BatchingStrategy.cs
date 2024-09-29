using Spam;

namespace Service.Batching;

public class BatchingStrategy(
    BatchDataLoader<string, string> batchDataLoader,
    int loops) : ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        var a = 1;
        for (int i = 0; i < loops; i++)
        {
            if (a == 1)
                a *= 2;
            else
                a /= 2;
        }
        await batchDataLoader.LoadAsync($"{context.RunnerIndex}-{context.CurrentExecution}", cancellationToken);
    }
}