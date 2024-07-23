using System.Text;
using Confluent.Kafka;
using Spam;

namespace Service;

public class KafkaProducerStrategy : ISpammerStrategy
{
    private readonly IProducer<string, string> _producer;
    private readonly string _name;
    private readonly bool _singletonTopic;
    private readonly string _message;
    
    public KafkaProducerStrategy(IProducer<string, string> producer, string name, bool singletonTopic, int size)
    {
        _producer = producer;
        _name = name;
        _singletonTopic = singletonTopic;
        var message = new StringBuilder("{");
        var i = 0;

        while (message.Length < size)
        {
            i++;
            message.Append($"\"property{i}\": \"value{i}\",");
        }
        message.Append("}");

        _message = message.ToString();
    }
    
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        if (_singletonTopic)
        {
            await _producer.ProduceAsync(_name, new Message<string, string>() { Value = _message }, cancellationToken);
        }
        else
        {
            await _producer.ProduceAsync($"{_name}-{context.RunnerIndex}",
                new Message<string, string>() { Value = _message }, cancellationToken);
        }
    }
}