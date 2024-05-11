using System.Diagnostics.Metrics;

namespace Service;

public class AppMetrics
{
    public static string MeterName = "MyAppTests";
    public Histogram<double> RunnerExecutionDuration { get; set; }

    public AppMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        RunnerExecutionDuration = meter.CreateHistogram<double>("runner_execution_duration_us");
    }
}