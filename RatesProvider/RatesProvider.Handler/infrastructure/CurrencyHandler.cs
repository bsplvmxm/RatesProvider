using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Interfaces;
using System.Timers;
using Microsoft.Extensions.Logging;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Handler.Infrastructure;

namespace RatesProvider.Handler;

public class CurrencyHandler : ICurrencyHandler
{
    private readonly IRatesBuilder _modelBuilder;
    private readonly IRabbitMQProducer _rabbitMQProducer;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IRetryPolicySettings _retryPolicySettings;
    private readonly ILogger _logger;
    private IHandleChecker _handleChecker;
    private IRatesGetter _currencyRecipient;
    private CurrencyRates _result;

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
        _result = new CurrencyRates();
    }

    public async Task HandleAsync(object? sender, ElapsedEventArgs e)
    {
        var retryPolicy = _retryPolicySettings.BuildRetryPolicy();

        _logger.LogInformation("Try Handle primary RatesGetter");

        _currencyRecipient = new PrimaryRatesGetter(_settingsProvider, _logger);
        _handleChecker = new PrimaryHandleChecker(_logger, _modelBuilder, retryPolicy);
        _result = await _handleChecker.Check(_currencyRecipient);

        _logger.LogInformation("Handle priamry RatesGetter ends with {0} elements in Dictionary", _result.Rates.Count);

        if (_result.Rates.Count == 0)
        {
            _logger.LogInformation("handle primary RatesGetter ends with 0 elements in Dictionary, Try Handle secondary RatesGetter");

            _currencyRecipient = new SecondaryRatesGetter(_settingsProvider, _logger);
            _handleChecker = new SecondaryHandleChecker(_logger, _modelBuilder, retryPolicy);
            _result = await _handleChecker.Check(_currencyRecipient);

            _logger.LogInformation("Handle secondary RatesGetter ends with {0} elements in Dictionary", _result.Rates.Count);
        }

        //Абстрактная фабрика

        _rabbitMQProducer.SendRatesMessage(_result);
    }
}