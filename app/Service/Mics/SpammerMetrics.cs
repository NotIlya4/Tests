using Spam;

namespace Service;

public class SpammerMetrics(
    AppMetrics metrics,
    string testName,
    string implementationClass) : ISpammerMetrics
{
    public void RecordExecutionProcessed(TimeSpan elapsed, RunnerExecutionContext executionContext)
    {
        metrics.RunnerExecutionDuration.Record(
            elapsed.Milliseconds,
            new KeyValuePair<string, object?>("test_name", testName),
            new KeyValuePair<string, object?>("runner_index", executionContext.RunnerIndex),
            new KeyValuePair<string, object?>("spammer_implementation_class", implementationClass));
    }
}