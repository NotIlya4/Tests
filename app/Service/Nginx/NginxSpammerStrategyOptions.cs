namespace Service;

public class NginxSpammerStrategyOptions
{
    public required NginxPingMode NginxPingMode { get; set; }
    public required NginxPingServiceFactory PingServiceFactory { get; set; }
}