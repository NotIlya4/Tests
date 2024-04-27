using System.Runtime.Serialization;
using Spam;

namespace Service;

[DataContract]
public class SpammerResultView
{
    [DataMember(Name = "preparationDuration")]
    public TimeSpan PreparationDuration { get; set; }

    [DataMember(Name = "runDuration")]
    public TimeSpan RunDuration { get; set; }

    public static SpammerResultView FromModel(SpammerRunResult result)
    {
        return new SpammerResultView()
        {
            PreparationDuration = result.PreparationDuration,
            RunDuration = result.RunDuration
        };
    }
}