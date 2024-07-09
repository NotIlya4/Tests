namespace Service;

public class NginxSpammerStrategyOptions
{
    public required NginxPingMode NginxPingMode { get; set; }
    public required NginxPingService NginxPingService { get; set; }
}