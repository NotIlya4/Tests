using Spam;

namespace Service;

public class SpammerMetrics(
    AppMetrics metrics,
    string testName,
    string implementationClass) : ISpammerMetrics
{
    public void RecordExecutionProcessed(TimeSpan elapsed, RunnerExecutionContext executionContext)
    {
        metrics.RunnerExecutionDuration.WithLabels(testName, executionContext.RunnerIndex.ToString())
            .Observe(elapsed.TotalMicroseconds);
    }
}