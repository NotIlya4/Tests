using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class SqlServerSpammerView : SmartDbContextSpammerOptionsView
{
    [DataMember(Name = "server")]
    public string? Server { get; set; }
}