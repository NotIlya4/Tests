using Spammer;

namespace Service;

public class NginxSpammerDependencies(
    NginxPing ping,
    IServiceProvider serviceProvider)
{
    public NginxPing Ping { get; set; } = ping;
    public IServiceProvider ServiceProvider { get; set; } = serviceProvider;
}