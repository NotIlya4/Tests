using System.Diagnostics.Metrics;

namespace Service;

public class AppMetrics
{
    public static string MeterName = "MyAppTests";
    public Histogram<float> RunnerExecutionDuration { get; set; }

    public AppMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        RunnerExecutionDuration = meter.CreateHistogram<float>("runner_execution_duration_ms");
    }
}