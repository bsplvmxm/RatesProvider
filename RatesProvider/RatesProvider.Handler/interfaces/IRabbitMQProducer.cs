using IncredibleBackendContracts.ExchangeModels;

namespace RatesProvider.Handler.Interfaces;

public interface IRabbitMQProducer
{
    Task SendRatesMessage<T>(T message);
}
