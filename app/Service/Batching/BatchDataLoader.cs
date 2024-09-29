using System.Diagnostics;

namespace Service.Batching;

public interface IBatchDataLoader<TKey, TEntity>
{
    Task<TEntity> LoadAsync(TKey key, CancellationToken cancellationToken);
}

public class BatchDataLoader<TKey, TEntity>(
    int maxBatchSize,
    TimeSpan linger,
    BatchDataLoaderDataProvider<TKey, TEntity> dataProvider,
    IDateTimeProvider dateTimeProvider,
    IWaitProvider waitProvider,
    IBatchingMetrics batchingMetrics,
    bool useYieldInsteadOfDelay) : IBatchDataLoader<TKey, TEntity>
{
    private BatchDataLoaderIndividualBatch<TKey, TEntity> _currentBatch = new(
        maxBatchSize,
        linger,
        dataProvider,
        dateTimeProvider,
        waitProvider,
        batchingMetrics,
        useYieldInsteadOfDelay);

    public async Task<TEntity> LoadAsync(TKey key, CancellationToken cancellationToken)
    {
        var mainLockStart = Stopwatch.GetTimestamp();
        BatchDataLoaderIndividualBatch<TKey, TEntity> currentBatch;
        lock (this)
        {
            currentBatch = _currentBatch;

            // Can't add key to batch if current batch is either executing, full or timed out, so create a new one
            if (!currentBatch.TryAddToBatch(key))
            {
                _currentBatch = BatchDataLoaderIndividualBatch<TKey, TEntity>.WithInitialKey(
                    key,
                    maxBatchSize,
                    linger,
                    dataProvider,
                    dateTimeProvider,
                    waitProvider,
                    batchingMetrics,
                    useYieldInsteadOfDelay);
                currentBatch = _currentBatch;
            }
        }
        batchingMetrics.ReportMainLock(Stopwatch.GetElapsedTime(mainLockStart));

        var currentBatchStart = Stopwatch.GetTimestamp();
        var result = await currentBatch.ExecuteAsync(cancellationToken);
        batchingMetrics.ReportCurrentBatchDurration(Stopwatch.GetElapsedTime(currentBatchStart));
        
        return result[key];
    }
}