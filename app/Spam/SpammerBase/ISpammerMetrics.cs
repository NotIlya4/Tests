namespace Spam;

public interface ISpammerMetrics
{
    void RecordExecutionProcessed(TimeSpan elapsed, RunnerExecutionContext executionContext);
}