using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;
using System.Timers;
using Microsoft.Extensions.Logging;
using Polly;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Handler.Infrastructure;

namespace RatesProvider.Handler;

public class CurrencyHandler : ICurrencyHandler
{
    private readonly IRatesBuilder _modelBuilder;
    private readonly IRabbitMQProducer _rabbitMQProducer;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger<CurrencyHandler> _logger;
    private readonly ILogger<PrimaryHandleChecker> _primaryHandleLogger;    
    private readonly ILogger<SecondaryHandleChecker> _secondaryHandleLogger;
    private readonly ILogger<PrimaryRatesGetter> _primaryRatesLogger;
    private readonly ILogger<SecondaryRatesGetter> _secondaryRatesLogger;
    private IHandleChecker _handleChecker;
    private IRatesGetter _currencyRecipient;
    private CurrencyRates _result;

    public CurrencyHandler(IRatesBuilder modelbuilder,
        ILogger<CurrencyHandler> logger,
        IRabbitMQProducer rabbitMQProducer,
        ISettingsProvider settingsProvider,
        ILogger<PrimaryRatesGetter> primaryRatesLogger,
        ILogger<SecondaryRatesGetter> secondaryRatesLogger,
        ILogger<PrimaryHandleChecker> primaryHandleLogger,
        ILogger<SecondaryHandleChecker> secondaryHandleLogger)
    {
        _settingsProvider = settingsProvider;
        _primaryRatesLogger = primaryRatesLogger;
        _secondaryRatesLogger = secondaryRatesLogger;
        _modelBuilder = modelbuilder;
        _result = new CurrencyRates();
        _logger = logger;
        _primaryHandleLogger = primaryHandleLogger;
        _secondaryHandleLogger = secondaryHandleLogger;
        _rabbitMQProducer = rabbitMQProducer;
    }

    public async Task HandleAsync(object? sender, ElapsedEventArgs e)
    {
        _logger.LogInformation("Try Handle primary RatesGetter");

        _currencyRecipient = new PrimaryRatesGetter(_settingsProvider, _primaryRatesLogger);
        _handleChecker = new PrimaryHandleChecker(_primaryHandleLogger, _modelBuilder);
        _result = await _handleChecker.Check(_currencyRecipient);

        _logger.LogInformation("Handle priamry RatesGetter ends with {0} elements in Dictionary", _result.Rates.Count);

        if (_result.Rates.Count == 0)
        {
            _logger.LogInformation("handle primary RatesGetter ends with 0 elements in Dictionary, Try Handle secondary RatesGetter");

            _currencyRecipient = new SecondaryRatesGetter(_settingsProvider, _secondaryRatesLogger);
            _handleChecker = new SecondaryHandleChecker(_secondaryHandleLogger, _modelBuilder);
            _result = await _handleChecker.Check(_currencyRecipient);

            _logger.LogInformation("Handle secondary RatesGetter ends with {0} elements in Dictionary", _result.Rates.Count);
        }

        _rabbitMQProducer.SendRatesMessage(_result);
    }
}