namespace Service.Batching;

public interface IDateTimeProvider
{
    /// <summary>Get current utc date time.</summary>
    DateTime GetUtcNow();
}

public class DefaultDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime GetUtcNow() => DateTime.UtcNow;
}