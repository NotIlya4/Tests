using Confluent.Kafka;
using Spam;

namespace Service;

public class KafkaProducerStrategy : ISpammerStrategy
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topicBaseName;
    private readonly int _topicPoolSize;
    private readonly bool _generateGuidKey;
    private readonly Func<RunnerExecutionContext, string> _messageProvider;
    private readonly TimeSpan _beginJitter;
    private bool _finishedBeginJitter = false;

    public KafkaProducerStrategy(IProducer<string, string> producer, string topicBaseName, int topicPoolSize, bool generateGuidKey,
        Func<RunnerExecutionContext, string> messageProvider, TimeSpan beginJitter)
    {
        _producer = producer;
        _topicBaseName = topicBaseName;
        _topicPoolSize = topicPoolSize;
        _generateGuidKey = generateGuidKey;
        _messageProvider = messageProvider;
        _beginJitter = beginJitter;
    }
    
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        if (!_finishedBeginJitter)
        {
            await Task.Delay(_beginJitter, cancellationToken);
            _finishedBeginJitter = true;
        }

        var msg = _messageProvider(context);
        await _producer.ProduceAsync($"{_topicBaseName}-{Random.Shared.Next(_topicPoolSize)}",
            new Message<string, string>() { Key = _generateGuidKey ? Guid.NewGuid().ToString() : null!, Value = msg }, cancellationToken);
    }
}