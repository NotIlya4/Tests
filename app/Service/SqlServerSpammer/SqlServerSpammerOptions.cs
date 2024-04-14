namespace Service;

public class SqlServerSpammerOptions
{
    public required int RunnerExecutions { get; init; }
    public required int ParallelRunners { get; init; }
    public required SqlServerSpammerEntityType EntityType { get; init; }
}