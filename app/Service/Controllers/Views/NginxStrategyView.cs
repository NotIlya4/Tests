using System.ComponentModel;
using System.Runtime.Serialization;

namespace Service;

public class NginxStrategyView : SpammerViewBase
{
    [DefaultValue(NginxPingMode.SingletonHttpClient)]
    public NginxPingMode PingMode { get; set; } = NginxPingMode.SingletonHttpClient;
}