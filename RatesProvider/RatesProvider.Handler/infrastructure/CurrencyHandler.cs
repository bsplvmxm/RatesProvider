using RatesProvider.Handler.Interfaces;
using System.Timers;
using Microsoft.Extensions.Logging;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Handler.Infrastructure;
using IncredibleBackendContracts.ExchangeModels;

namespace RatesProvider.Handler;

public class CurrencyHandler : ICurrencyHandler
{
    private readonly IRabbitMQProducer _rabbitMQProducer;
    private readonly ILogger _logger;
    private List<IRatesSourceHandler> _ratesSourceHandlers;
    private CurrencyRate _result;

    public CurrencyHandler(IRatesBuilder ratesBuilder,
        ILogger<CurrencyHandler> logger,
        IRabbitMQProducer rabbitMQProducer,
        ISettingsProvider settingsProvider,
        IRetryPolicySettings retryPolicySettings)
    {
        _rabbitMQProducer = rabbitMQProducer;
        _logger = logger;
        _result = new CurrencyRate();
        _ratesSourceHandlers = new List<IRatesSourceHandler>()
        {
            new PrimarySourceHandler(_logger, settingsProvider, ratesBuilder, retryPolicySettings.BuildRetryPolicy()),
            new SecondarySourceHandler(_logger, settingsProvider, ratesBuilder, retryPolicySettings.BuildRetryPolicy())
        };
    }

    public async Task HandleAsync(object? sender, ElapsedEventArgs e)
    {
        foreach (var ratesSourceHandler in _ratesSourceHandlers)
        {
            _result = await ratesSourceHandler.Handle();

            if(_result.Rates.Count != 0)
            {
                _logger.LogInformation("Finish handle with {0} elements in dictionary", _result.Rates.Count);
                break;
            }
        }

        await _rabbitMQProducer.SendRatesMessage(_result);
    }
}