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
            new HistogramConfiguration() { Buckets = GetBoundaries(100, 10_000_000) });
    }
    
    private static double[] GetBoundaries(double from, double to)
    {
        var current = from;
        List<double> items = [];
        while (to > current)
        {
            items.Add(current);
            current = Math.Round(current * 1.5);
        }
        
        items.Add(current);

        return items.ToArray();
    }
}