using System.ComponentModel;

namespace Service;

public class PostgresStrategyOptionsView : DbContextStrategyOptionsView
{
    [DefaultValue(false)]
    public bool IsDbContextStrategy { get; set; }

    public PostgresStrategyType PostgresStrategyType { get; set; }

    [DefaultValue(0)]
    public int ThrottleMs { get; set; }
}