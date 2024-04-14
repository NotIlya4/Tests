using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class NginxSpammerView : SpammerBaseView
{
    [DataMember(Name = "pingMode")]
    public NginxPingMode PingMode { get; set; }
}