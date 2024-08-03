using Prometheus;

namespace Service;

public class AppMetrics
{
    public static string MeterName = "MyAppTests";
    public static string RunnerExecutionDurationName = "runner_execution_duration_us";
    
    public Histogram RunnerExecutionDuration { get; set; }

    public AppMetrics()
    {
        RunnerExecutionDuration = Metrics.CreateHistogram(
            RunnerExecutionDurationName,
            "asd",
            ["test_name", "runner_index"],
            new HistogramConfiguration { Buckets = CreateBoundaries() });
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