namespace Service.Batching;

public class BatchDataLoaderDataProvider<TKey, TEntity>
{
    public Func<IEnumerable<TKey>, IReadOnlyDictionary<TKey, TEntity>>? SyncFunc { get; }
    public Func<IEnumerable<TKey>, CancellationToken, Task<IReadOnlyDictionary<TKey, TEntity>>>? AsyncFunc { get; }

    private BatchDataLoaderDataProvider(
        Func<IEnumerable<TKey>, IReadOnlyDictionary<TKey, TEntity>>? syncFunc,
        Func<IEnumerable<TKey>, CancellationToken, Task<IReadOnlyDictionary<TKey, TEntity>>>? asyncFunc)
    {
        if (syncFunc is null && asyncFunc is null)
            throw new InvalidOperationException("Either sync or async func must be provided");
        SyncFunc = syncFunc;
        AsyncFunc = asyncFunc;
    }

    public static BatchDataLoaderDataProvider<TKey, TEntity> FromSyncFunc(
        Func<IEnumerable<TKey>, IReadOnlyDictionary<TKey, TEntity>> syncFunc)
    {
        return new BatchDataLoaderDataProvider<TKey, TEntity>(syncFunc, null);
    }

    public static BatchDataLoaderDataProvider<TKey, TEntity> FromAsyncFunc(
        Func<IEnumerable<TKey>, CancellationToken, Task<IReadOnlyDictionary<TKey, TEntity>>> asyncFunc)
    {
        return new BatchDataLoaderDataProvider<TKey, TEntity>(null, asyncFunc);
    }
}