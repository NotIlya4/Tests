using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class SpammerBaseView
{
    [DataMember(Name = "parallelRunners")]
    public int ParallelRunners { get; set; }
    
    [DataMember(Name = "runnerExecutions")]
    public int RunnerExecutions { get; set; }

    [DataMember(Name = "testName")]
    public string TestName { get; set; } = null!;
}