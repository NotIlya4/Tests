using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Spammer;

public class SpammerOptions
{
    public required int RunnerExecutions { get; init; }
    public required int ParallelRunners { get; init; }
    public ISpammerMetrics? Metrics { get; init; }
    public ILogger<SpammerBase>? Logger { get; init; }
}