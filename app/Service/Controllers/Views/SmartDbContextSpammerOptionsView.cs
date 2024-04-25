using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class SmartDbContextSpammerOptionsView : SpammerBaseView
{
    [DataMember(Name = "spammerStrategyType")]
    public SpammerStrategyType SpammerStrategyType { get; set; }
}