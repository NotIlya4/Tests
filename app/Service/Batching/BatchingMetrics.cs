namespace Service.Batching;

public interface IBatchingMetrics
{
    void ReportInnerLatency(TimeSpan timeSpan);
    void ReportWaitLatency(TimeSpan timeSpan);
    void ReportMainLock(TimeSpan timeSpan);
    void ReportInnerWaitLatency(TimeSpan timeSpan);
    void ReportStrategySleep(TimeSpan timeSpan);
    void ReportCurrentBatchDurration(TimeSpan timeSpan);
}

public class BatchingMetrics(AppMetrics appMetrics, string testName) : IBatchingMetrics
{
    public void ReportInnerLatency(TimeSpan timeSpan)
    {
        appMetrics.BatchingInternalDuration.WithLabels(testName).Observe(timeSpan.TotalMilliseconds);
    }

    public void ReportWaitLatency(TimeSpan timeSpan)
    {
        appMetrics.BatchingWaitDuration.WithLabels(testName).Observe(timeSpan.TotalMilliseconds);
    }

    public void ReportMainLock(TimeSpan timeSpan)
    {
        appMetrics.BatchingMainLockDuration.WithLabels(testName).Observe(timeSpan.TotalMilliseconds);
    }

    public void ReportInnerWaitLatency(TimeSpan timeSpan)
    {
        appMetrics.BatchingInnerWaitDuration.WithLabels(testName).Observe(timeSpan.TotalMilliseconds);
    }

    public void ReportStrategySleep(TimeSpan timeSpan)
    {
        appMetrics.BatchingStrategySleepDuration.WithLabels(testName).Observe(timeSpan.TotalMilliseconds);
    }

    public void ReportCurrentBatchDurration(TimeSpan timeSpan)
    {
        appMetrics.BatchingCurrentBatchDuration.WithLabels(testName).Observe(timeSpan.TotalMilliseconds);
    }
}