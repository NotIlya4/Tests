namespace Service.Batching;

public interface IWaitProvider
{
    Task WaitAsync(TimeSpan delay, CancellationToken cancellationToken);
}

public class WaitProvider : IWaitProvider
{
    public async Task WaitAsync(TimeSpan delay, CancellationToken cancellationToken) =>
        await Task.Delay(delay, cancellationToken);
}