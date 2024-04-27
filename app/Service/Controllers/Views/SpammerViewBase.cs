using System.ComponentModel;
using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class SpammerViewBase
{
    [DataMember(Name = "parallelRunners")]
    [DefaultValue(1)]
    public int ParallelRunners { get; set; } = 1;

    [DataMember(Name = "runnerExecutions")]
    [DefaultValue(1)]
    public int RunnerExecutions { get; set; } = 1;

    [DataMember(Name = "testName")]
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