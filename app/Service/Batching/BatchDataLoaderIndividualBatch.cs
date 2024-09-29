using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace Service.Batching;

public class BatchDataLoaderIndividualBatch<TKey, TEntity>(
    int maxBatchSize,
    TimeSpan linger,
    BatchDataLoaderDataProvider<TKey, TEntity> dataProvider,
    IDateTimeProvider dateTimeProvider,
    IWaitProvider waitProvider,
    IBatchingMetrics batchingMetrics,
    bool useYieldInsteadOfDelay)
{
    private readonly ISet<TKey> _keys = new HashSet<TKey>();

    private readonly long _batchStart = Stopwatch.GetTimestamp();

    private bool _isClosed;
    private readonly TaskCompletionSource _earlyClose = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private Task<IReadOnlyDictionary<TKey, TEntity>>? _resultTask;
    private readonly object _resultTaskSync = new();

    public static BatchDataLoaderIndividualBatch<TKey, TEntity> WithInitialKey(
        TKey key,
        int maxBatchSize,
        TimeSpan linger,
        BatchDataLoaderDataProvider<TKey, TEntity> dataProvider,
        IDateTimeProvider dateTimeProvider,
        IWaitProvider waitProvider,
        IBatchingMetrics batchingMetrics,
        bool useYieldInsteadOfDelay)
    {
        var batch = new BatchDataLoaderIndividualBatch<TKey, TEntity>(
            maxBatchSize,
            linger,
            dataProvider,
            dateTimeProvider,
            waitProvider,
            batchingMetrics,
            useYieldInsteadOfDelay);
        batch._keys.Add(key);

        return batch;
    }

    public bool TryAddToBatch(TKey key)
    {
        lock (this)
        {
            if (_isClosed)
                return false;

            if (_keys.Count >= maxBatchSize || Stopwatch.GetElapsedTime(_batchStart) > linger)
            {
                EnsureClosedFast();
                return false;
            }

            _keys.Add(key);

            if (_keys.Count >= maxBatchSize)
            {
                EnsureClosedFast();
            }

            return true;
        }


        void EnsureClosedFast()
        {
            _isClosed = true;
            _earlyClose.TrySetResult();
        }
    }

    public async Task<IReadOnlyDictionary<TKey, TEntity>> ExecuteAsync(CancellationToken cancellationToken)
    {
        lock (_resultTaskSync)
        {
            _resultTask ??= ExecuteInternalAsync(cancellationToken);
        }

        return await _resultTask;
    }

    private async Task<IReadOnlyDictionary<TKey, TEntity>> ExecuteInternalAsync(CancellationToken cancellationToken)
    {
        var waitStart = Stopwatch.GetTimestamp();
        if (useYieldInsteadOfDelay)
            await Task.Yield();
        else
        {
            await Task.WhenAny(
                waitProvider.WaitAsync(
                    TimeSpan.FromMilliseconds(Math.Max((linger - Stopwatch.GetElapsedTime(_batchStart)).TotalMilliseconds, 1)),
                    cancellationToken),
                _earlyClose.Task);
        }
        batchingMetrics.ReportWaitLatency(Stopwatch.GetElapsedTime(waitStart));

        EnsureClosed();

        var start = Stopwatch.GetTimestamp();
        IReadOnlyDictionary<TKey, TEntity> result;
        if (dataProvider.SyncFunc is not null)
            result = dataProvider.SyncFunc(_keys);
        else
            result = await dataProvider.AsyncFunc!(_keys, cancellationToken);
        batchingMetrics.ReportInnerLatency(Stopwatch.GetElapsedTime(start));
        return result;
    }

    private void EnsureClosed()
    {
        lock (this)
        {
            _isClosed = true;
        }
    }
}