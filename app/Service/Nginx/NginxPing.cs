namespace Service;

public class NginxPing(HttpClient client, string nginxUrl)
{
    public async Task Ping()
    {
        var r = await client.GetAsync(nginxUrl);
        var data = await r.Content.ReadAsStringAsync();
    }
}