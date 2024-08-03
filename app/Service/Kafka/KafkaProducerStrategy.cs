using Confluent.Kafka;
using Spam;

namespace Service;

public class KafkaProducerStrategy : ISpammerStrategy
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topicBaseName;
    private readonly bool _singletonTopic;
    private readonly Func<RunnerExecutionContext, string> _messageProvider;
    private readonly TimeSpan _beginJitter;
    private bool _finishedBeginJitter = false;

    public KafkaProducerStrategy(IProducer<string, string> producer, string topicBaseName, bool singletonTopic,
        Func<RunnerExecutionContext, string> messageProvider, TimeSpan beginJitter)
    {
        _producer = producer;
        _topicBaseName = topicBaseName;
        _singletonTopic = singletonTopic;
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
        if (_singletonTopic)
        {
            await _producer.ProduceAsync(_topicBaseName, new Message<string, string>() { Value = msg }, cancellationToken);
        }
        else
        {
            await _producer.ProduceAsync($"{_topicBaseName}-{context.RunnerIndex}",
                new Message<string, string>() { Value = msg }, cancellationToken);
        }
    }
}