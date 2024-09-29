using Prometheus;

namespace Service;

public class AppMetrics
{
    public static string MeterName = "MyAppTests";
    public static string RunnerExecutionDurationName = "runner_execution_duration_us";
    public static string BatchingInternalDurationName = "batching_internal_duration_ms";
    public static string BatchingWaitDurationName = "batching_wait_duration_ms";
    public static string BatchingInnerWaitDurationName = "batching_internal_wait_duration_ms";
    public static string BatchingMainLockDurationName = "batching_main_lock_duration_ms";
    public static string BatchingStrategySleepDurationName = "batching_strategy_sleep_duration_ms";
    public static string BatchingCurrentBatchDurationName = "batching_current_batch_duration_ms";
    
    public Histogram RunnerExecutionDuration { get; set; }
    
    public Histogram BatchingInternalDuration { get; set; }
    public Histogram BatchingWaitDuration { get; set; }
    public Histogram BatchingInnerWaitDuration { get; set; }
    public Histogram BatchingMainLockDuration { get; set; }
    public Histogram BatchingStrategySleepDuration { get; set; }
    public Histogram BatchingCurrentBatchDuration { get; set; }

    public AppMetrics()
    {
        RunnerExecutionDuration = Metrics.CreateHistogram(
            RunnerExecutionDurationName,
            "asd",
            ["test_name", "runner_index"],
            new HistogramConfiguration { Buckets = CreateBoundaries() });

        BatchingInternalDuration = Metrics.CreateHistogram(BatchingInternalDurationName, "asd", ["test_name"],
            new HistogramConfiguration() { Buckets = GetLinearBoundaries(0, 5000, 100).ToArray() });
        BatchingWaitDuration = Metrics.CreateHistogram(BatchingWaitDurationName, "asd", ["test_name"],
            new HistogramConfiguration() { Buckets = GetLinearBoundaries(0, 5000, 100).ToArray() });
        BatchingInnerWaitDuration = Metrics.CreateHistogram(BatchingInnerWaitDurationName, "asd", ["test_name"],
            new HistogramConfiguration() { Buckets = GetLinearBoundaries(0, 5000, 100).ToArray() });
        BatchingMainLockDuration = Metrics.CreateHistogram(BatchingMainLockDurationName, "asd", ["test_name"],
            new HistogramConfiguration() { Buckets = GetLinearBoundaries(0, 5000, 100).ToArray() });
        BatchingStrategySleepDuration = Metrics.CreateHistogram(BatchingStrategySleepDurationName, "asd", ["test_name"],
            new HistogramConfiguration() { Buckets = GetLinearBoundaries(0, 5000, 100).ToArray() });
        BatchingCurrentBatchDuration = Metrics.CreateHistogram(BatchingCurrentBatchDurationName, "asd", ["test_name"],
            new HistogramConfiguration() { Buckets = GetLinearBoundaries(0, 5000, 100).ToArray() });
    }

    public static double[] CreateBoundaries()
    {
        return GetLinearBoundaries(0, 1_000, 10)
            .Concat(GetLinearBoundaries(1_000, 10_000, 100))
            .Concat(GetLinearBoundaries(10_000, 100_000, 1_000))
            .Concat(GetLinearBoundaries(100_000, 1_000_000, 10_000))
            .Concat(GetLinearBoundaries(1_000_000, 10_000_000, 100_000))
            .ToArray();
    }

    private static IEnumerable<double> GetLinearBoundaries(int from, int to, int step)
    {
        var current = from;

        while (current < to)
        {
            yield return current;
            current += step;
        }
    }
}