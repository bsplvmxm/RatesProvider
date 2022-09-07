using RatesProvider.Handler.Interfaces;
using System.Timers;
using Microsoft.Extensions.Logging;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Handler.Infrastructure;
using IncredibleBackendContracts.ExchangeModels;

namespace RatesProvider.Handler;

public class CurrencyHandler : ICurrencyHandler
{
    private readonly IRatesBuilder _modelBuilder;
    private readonly IRabbitMQProducer _rabbitMQProducer;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IRetryPolicySettings _retryPolicySettings;
    private readonly ILogger _logger;
    private HandleFactory _handleFactory;
    private CurrencyRate _result;

    public CurrencyHandler(IRatesBuilder modelbuilder,
        ILogger<CurrencyHandler> logger,
        IRabbitMQProducer rabbitMQProducer,
        ISettingsProvider settingsProvider,
        IRetryPolicySettings retryPolicySettings)
    {
        _retryPolicySettings = retryPolicySettings;
        _rabbitMQProducer = rabbitMQProducer;
        _modelBuilder = modelbuilder;
        _settingsProvider = settingsProvider;
        _logger = logger;
        _result = new CurrencyRate();
    }

    public async Task HandleAsync(object? sender, ElapsedEventArgs e)
    {
        var retryPolicy = _retryPolicySettings.BuildRetryPolicy();

        _logger.LogInformation("Try Handle primary RatesGetter");

        _handleFactory = new PrimaryHandler(_logger, _settingsProvider, _modelBuilder, retryPolicy); //PrimarySourceHandler //ratesSourceHandler(_handleFactory)
        _result = await _handleFactory.Handle();

        _logger.LogInformation("Handle priamry RatesGetter ends with {0} elements in Dictionary", _result.Rates.Count);

        if (_result.Rates.Count == 0)
        {
            _logger.LogInformation("handle primary RatesGetter ends with 0 elements in Dictionary, Try Handle secondary RatesGetter");

            _handleFactory = new SecondaryHandler(_logger, _settingsProvider, _modelBuilder, retryPolicy);
            _result = await _handleFactory.Handle();

            _logger.LogInformation("Handle secondary RatesGetter ends with {0} elements in Dictionary", _result.Rates.Count);
        }
        _logger.LogInformation("Send rates to Queue");
        await _rabbitMQProducer.SendRatesMessage(_result);
    }
}