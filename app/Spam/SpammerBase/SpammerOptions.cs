﻿using Microsoft.Extensions.Logging;

namespace Spam;

public class SpammerOptions
{
    public required int RunnerExecutions { get; init; }
    public required int ParallelRunners { get; init; }
    public required ISpammerStrategy SpammerStrategy { get; init; }
    public required ISpammerParallelEngine SpammerParallelEngine { get; init; }
    public ISpammerMetrics? Metrics { get; init; }
    public ILogger<Spammer>? Logger { get; init; }
}