namespace Service;

public class NginxPingServiceFactory(IServiceProvider serviceProvider)
{
    public NginxPingService Create()
    {
        return serviceProvider.GetRequiredService<NginxPingService>();
    }
}