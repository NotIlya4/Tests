using System.ComponentModel;
using System.Runtime.Serialization;

namespace Service;

public class SpammerViewBase
{
    [DefaultValue(1)]
    public int ParallelRunners { get; set; } = 1;

    [DefaultValue(1)]
    public int RunnerExecutions { get; set; } = 1;

    [DefaultValue("test1")]
    public string TestName { get; set; } = "test1";

    public virtual void ApplyBuilder(SpammerBuilder builder)
    {
        builder
            .WithParallelRunners(ParallelRunners)
            .WithRunnerExecutions(RunnerExecutions)
            .WithTestName(TestName);
    }
}