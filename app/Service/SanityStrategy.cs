using Spam;

namespace Service;

public class SanityStrategy(int sleepMs, int rareSleepMs) : ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        if (Random.Shared.Next(99) > 96)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(rareSleepMs), cancellationToken);
        }
        else
        {
            await Task.Delay(TimeSpan.FromMilliseconds(sleepMs), cancellationToken);
        }
    }
}