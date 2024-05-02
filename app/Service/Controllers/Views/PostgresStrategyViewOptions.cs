using System.ComponentModel;

namespace Service;

public class PostgresStrategyViewOptions : DbContextStrategyViewOptions
{
    [DefaultValue(false)]
    public bool Dapper { get; set; }
}