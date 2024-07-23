using System.ComponentModel;
using Confluent.Kafka;

namespace Service;

public class KafkaProducerStrategyOptionsView
{
    public SpammerOptionsView SpammerOptionsView { get; set; } = new SpammerOptionsView();

    [DefaultValue(true)]
    public bool SingletonTopic { get; set; } = true;

    [DefaultValue(true)]
    public bool SingletonProducer { get; set; }
    
    [DefaultValue(1024 * 1024)]
    public int Size { get; set; } = 1024 * 1024;
    
    [DefaultValue(CompressionType.Snappy)]
    public CompressionType CompressionType { get; set; } = CompressionType.Snappy;
    
    [DefaultValue(Acks.All)]
    public Acks Acks { get; set; } = Acks.All;
    
    [DefaultValue(false)]
    public bool EnableIdempotence { get; set; } = true;
}