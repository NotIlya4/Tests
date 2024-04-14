using Spammer;

namespace Service;

public class SpammerBuilderBaseDependencies(
    ILogger<SpammerBase> logger,
    AppMetrics metrics)
{
    public ILogger<SpammerBase> Logger { get; set; } = logger;
    public AppMetrics Metrics { get; set; } = metrics;
}