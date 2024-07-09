using System.ComponentModel;
using System.Runtime.Serialization;

namespace Service;

public class NginxStrategyOptionsView : SpammerOptionsView
{
    public SpammerOptionsView SpammerOptions { get; set; }

    [DefaultValue(NginxPingMode.SingletonHttpClient)]
    public NginxPingMode PingMode { get; set; } = NginxPingMode.SingletonHttpClient;
}