using Confluent.Kafka;
using Service.Prometheus;

namespace Service;

public class KafkaProducerContainer
{
    private readonly Func<string, ProducerConfig> _configProvider;
    private readonly bool _singleton;
    private readonly Lazy<IProducer<string, string>> _singletonProducer;

    public KafkaProducerContainer(Func<string, ProducerConfig> configProvider, bool singleton)
    {
        _configProvider = configProvider;
        _singleton = singleton;
        _singletonProducer = new Lazy<IProducer<string, string>>(() => CreateProducerCore("singleton"));
    }

    public IProducer<string, string> GetProducer(int runnerIndex)
    {
        if (_singleton)
        {
            return _singletonProducer.Value;
        }

        return CreateProducerCore(runnerIndex.ToString());
    }

    private IProducer<string, string> CreateProducerCore(string runnerIdentity)
    {
        return new ProducerBuilder<string, string>(_configProvider(runnerIdentity)).HandleStatistics(
            new PrometheusProducerStatisticsHandler(new string[] { "application", "librdkafka_type" }, 
                new string[] { Environment.GetEnvironmentVariable("HOSTNAME") ?? "producer", "producer" })).Build();
    }
}