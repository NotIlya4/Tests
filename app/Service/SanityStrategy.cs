using Spam;

namespace Service;

public class SanityStrategy(int sleepMs) : ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(sleepMs), cancellationToken);
    }
}