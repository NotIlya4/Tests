using System.ComponentModel;
using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class NginxStrategyView : SpammerViewBase
{
    [DataMember(Name = "pingMode")]
    [DefaultValue(NginxPingMode.SingletonHttpClient)]
    public NginxPingMode PingMode { get; set; } = NginxPingMode.SingletonHttpClient;
}