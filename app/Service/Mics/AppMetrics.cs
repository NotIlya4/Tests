using System.Diagnostics.Metrics;

namespace Service;

public class AppMetrics
{
    public static string MeterName = "MyAppTests";
    public static string RunnerExecutionDurationName = "runner_execution_duration_us";
    
    public Histogram<double> RunnerExecutionDuration { get; set; }

    public AppMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        RunnerExecutionDuration = meter.CreateHistogram<double>(RunnerExecutionDurationName);
    }
}