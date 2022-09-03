namespace RatesProvider.Handler.Interfaces;

public interface IRabbitMQProducer
{
    void SendRatesMessage<T>(T message);
}
