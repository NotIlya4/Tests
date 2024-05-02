using System.ComponentModel;
using System.Runtime.Serialization;
using Service.Enums;

namespace Service;

public class DbContextStrategyViewOptions : SpammerViewBase
{
    [DefaultValue(DbContextStrategyType.StringEntity)]
    public DbContextStrategyType DbContextStrategyType { get; set; } = DbContextStrategyType.StringEntity;

    [DefaultValue(DataCreationStrategyType.Guid)]
    public DataCreationStrategyType DataCreationStrategyType { get; set; } = DataCreationStrategyType.Guid;
    
    [DefaultValue(128)]
    public int FixedLengthStringLength { get; set; } = 128;
}