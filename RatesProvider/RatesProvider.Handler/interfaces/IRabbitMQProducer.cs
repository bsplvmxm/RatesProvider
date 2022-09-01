namespace RatesProvider.Handler.Interfaces;

public interface IRabbitMQProducer
{
    void SendProductMessage<T>(T message);
}
