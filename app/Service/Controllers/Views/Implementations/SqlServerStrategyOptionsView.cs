using System.ComponentModel;
using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class SqlServerStrategyOptionsView : DbContextStrategyOptionsView
{
    [DataMember(Name = "server")]
    [DefaultValue(null)]
    public string? Server { get; set; }
}