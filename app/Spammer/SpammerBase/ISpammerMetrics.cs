namespace Spammer;

public interface ISpammerMetrics
{
    void RecordExecutionProcessed(TimeSpan elapsed, RunnerExecutionContext executionContext);
}