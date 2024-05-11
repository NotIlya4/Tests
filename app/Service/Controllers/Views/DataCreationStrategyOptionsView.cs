using System.ComponentModel;
using Service.Enums;
using Spam;

namespace Service;

public class DataCreationStrategyOptionsView
{
    public DataCreationStrategyType DataCreationStrategyType { get; set; }

    [DefaultValue(128)]
    public int FixedLengthStringLength { get; set; } = 128;
    
    public ISimpleDataCreationStrategy<string> CreateStrategy()
    {
        return DataCreationStrategyType.CreateStrategy(FixedLengthStringLength);
    }
}