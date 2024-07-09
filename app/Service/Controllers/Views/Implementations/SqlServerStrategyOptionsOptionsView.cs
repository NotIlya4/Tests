using System.ComponentModel;
using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class SqlServerStrategyOptionsOptionsView
{
    public SpammerOptionsView SpammerOptions { get; set; }
    public DbContextStrategyOptionsView DbContextStrategyOptions { get; set; }
    public string? Server { get; set; }
}