using Spam;

namespace Service.Enums;

public static class DataCreationStrategyTypeExtensions
{
    public static ISimpleDataCreationStrategy<string> CreateStrategy(this DataCreationStrategyType type, int fixedLengthStringLength)
    {
        return type switch
        {
            DataCreationStrategyType.Guid => new GuidDataCreationStrategy(),
            DataCreationStrategyType.FixedLengthString => new FixedLengthStringDataCreationStrategy(
                fixedLengthStringLength),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}