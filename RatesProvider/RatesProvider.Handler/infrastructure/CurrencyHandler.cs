using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;
using System.Timers;
using Microsoft.Extensions.Logging;
using Polly;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler;

public class CurrencyHandler : ICurrencyHandler
{
    private readonly IRatesBuilder _modelBuilder;
    private readonly IRabbitMQProducer _rabbitMQProducer;
    private readonly ILogger<CurrencyHandler> _logger;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger<PrimaryRatesGetter> _primaryRatesLogger;
    private readonly ILogger<SecondaryRatesGetter> _secondaryRatesLogger;
    private IRatesGetter _currencyRecipient;
    private CurrencyRates _result;

    public CurrencyHandler(IRatesBuilder modelbuilder,
        ILogger<CurrencyHandler> logger,
        IRabbitMQProducer rabbitMQProducer,
        ISettingsProvider settingsProvider,
        ILogger<PrimaryRatesGetter> primaryRatesLogger,
        ILogger<SecondaryRatesGetter> secondaryRatesLogger)
    {
        _settingsProvider = settingsProvider;
        _primaryRatesLogger = primaryRatesLogger;
        _secondaryRatesLogger = secondaryRatesLogger;
        _modelBuilder = modelbuilder;
        _result = new CurrencyRates();
        _logger = logger;
        _rabbitMQProducer = rabbitMQProducer;
    }

    public async Task HandleAsync(object? sender, ElapsedEventArgs e)
    {
        try
        {
            _logger.LogInformation("Try handle primary api's response");

            _currencyRecipient = new PrimaryRatesGetter(_settingsProvider, _primaryRatesLogger);

            var passedCurrencyPairs = await Policy.Handle<Exception>()
              .Retry(3, (e, i) => _logger.LogInformation(e.Message))
              .Execute(_currencyRecipient.GetRates);

            _result.Rates = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs).Quotes;
            foreach (var rate in _result.Rates)
            {
                Console.WriteLine($"{rate.Key}:{rate.Value}");
            }

        }
        catch (Exception ex)
        {
            if (ex is RatesBuildException || ex is HttpRequestException)
            {
                _logger.LogInformation("Try handle secondary api's response");

                var passedCurrencyPairs = await Policy.Handle<Exception>()
              .Retry(3, (e, i) => _logger.LogInformation(e.Message))
              .Execute(_currencyRecipient.GetRates);

                _result.Rates = _modelBuilder.ConvertToDecimal(_modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs).Data);
            }
            else
            { 
                _logger.LogInformation("Unprocessable response: {0}", ex);
            }
        }

        _rabbitMQProducer.SendRatesMessage(_result);
        _logger.LogInformation("Handle success");
    }
}
