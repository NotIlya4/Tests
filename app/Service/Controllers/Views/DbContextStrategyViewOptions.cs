using System.ComponentModel;
using System.Runtime.Serialization;
using Service.Enums;

namespace Service;

[DataContract]
public class DbContextStrategyViewOptions : SpammerViewBase
{
    [DataMember(Name = "spammerStrategyType")]
    [DefaultValue(DbContextStrategyType.StringEntity)]
    public DbContextStrategyType DbContextStrategyType { get; set; } = DbContextStrategyType.StringEntity;

    [DataMember(Name = "dataCreationStrategy")]
    [DefaultValue(DataCreationStrategyType.Guid)]
    public DataCreationStrategyType DataCreationStrategyType { get; set; } = DataCreationStrategyType.Guid;
    
    [DataMember(Name = "fixedLengthStringLength")]
    [DefaultValue(128)]
    public int FixedLengthStringLength { get; set; } = 128;
}