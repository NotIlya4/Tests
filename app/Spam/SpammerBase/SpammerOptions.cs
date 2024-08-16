using Microsoft.Extensions.Logging;

namespace Spam;

public class SpammerOptions
{
    public required string TestName { get; init; }
    public required int RunnerExecutions { get; init; }
    public required int ParallelRunners { get; init; }
    public required SpammerStrategyFactory SpammerStrategyFactory { get; init; }
    public required ISpammerParallelEngine SpammerParallelEngine { get; init; }
    public ISpammerMetrics? Metrics { get; init; }
    public ILogger<Spammer>? Logger { get; init; }
}

public delegate Task<ISpammerStrategy> SpammerStrategyFactory(int parallelRunnerIndex, CancellationToken ct);