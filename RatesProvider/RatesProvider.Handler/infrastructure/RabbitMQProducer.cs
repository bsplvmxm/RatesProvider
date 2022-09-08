using RatesProvider.Handler.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace RatesProvider.Handler.Infrastructure;

public class RabbitMQProducer : IRabbitMQProducer
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger _logger;

    public RabbitMQProducer(ILogger<RabbitMQProducer> logger, IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task SendRatesMessage<T>(T message)
    {
        try
        {
            _logger.LogInformation("Send rates to Queue");
            await _publishEndpoint.Publish(message!);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
