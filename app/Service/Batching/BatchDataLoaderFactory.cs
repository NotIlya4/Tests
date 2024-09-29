namespace Service.Batching;

public interface IBatchDataLoaderFactory
{
    BatchDataLoader<TKey, TEntity> Create<TKey, TEntity>(
        int maxBatchSize,
        TimeSpan linger,
        string testName,
        bool useYieldInsteadOfDelay,
        BatchDataLoaderDataProvider<TKey, TEntity> dataProvider);
}

public class BatchDataLoaderFactory(
    IDateTimeProvider dateTimeProvider,
    IWaitProvider waitProvider,
    AppMetrics appMetrics) : IBatchDataLoaderFactory
{
    public BatchDataLoader<TKey, TEntity> Create<TKey, TEntity>(
        int maxBatchSize,
        TimeSpan linger,
        string testName,
        bool useYieldInsteadOfDelay,
        BatchDataLoaderDataProvider<TKey, TEntity> dataProvider)
    {
        return new BatchDataLoader<TKey, TEntity>(
            maxBatchSize,
            linger,
            dataProvider,
            dateTimeProvider,
            waitProvider,
            new BatchingMetrics(appMetrics, testName),
            useYieldInsteadOfDelay);
    }
}