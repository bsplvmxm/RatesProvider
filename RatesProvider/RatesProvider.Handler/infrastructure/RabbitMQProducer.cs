using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using MassTransit;
using IncredibleBackendContracts.ExchangeModels;
using Newtonsoft.Json;

namespace RatesProvider.Handler.Infrastructure;

public class RabbitMQProducer : IRabbitMQProducer
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMQProducer(ISettingsProvider settingsPovider, IPublishEndpoint publishEndpoint)
    {
        _settingsProvider = settingsPovider;
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendRatesMessage<T>(T message)
    {
        if (message == null)
            return;

        await _publishEndpoint.Publish(message);
    }
}
