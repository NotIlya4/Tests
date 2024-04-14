namespace Spammer;

public class RunnerExecutionContext
{
    public required int CurrentExecution { get; init; }
    public required int RunnerIndex { get; init; }
    public required Dictionary<object, object> Data { get; init; }
}