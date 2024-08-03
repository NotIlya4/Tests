using Confluent.Kafka;

namespace Service;

public class KafkaProducerContainer
{
    private readonly ProducerConfig _config;
    private readonly bool _singleton;
    private readonly Lazy<IProducer<string, string>> _singletonProducer;

    public KafkaProducerContainer(ProducerConfig config, bool singleton)
    {
        _config = config;
        _singleton = singleton;
        _singletonProducer = new Lazy<IProducer<string, string>>(CreateProducerCore);
    }

    public IProducer<string, string> GetProducer()
    {
        if (_singleton)
        {
            return _singletonProducer.Value;
        }

        return CreateProducerCore();
    }

    private IProducer<string, string> CreateProducerCore()
    {
        return new ProducerBuilder<string, string>(_config).Build();
    }
}