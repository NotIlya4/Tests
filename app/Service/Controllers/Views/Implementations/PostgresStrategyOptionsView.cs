using System.ComponentModel;

namespace Service;

public class PostgresStrategyOptionsView : DbContextStrategyOptionsView
{
    [DefaultValue(false)]
    public bool IsDbContextStrategy { get; set; }

    public PostgresStrategyType PostgresStrategyType { get; set; }

    public SelectStrategyType SelectStrategyType { get; set; }
    
    public int Limit { get; set; }
}