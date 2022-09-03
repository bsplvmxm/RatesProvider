using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;
using System.Timers;
using Microsoft.Extensions.Logging;
using Polly;

namespace RatesProvider.Handler;

public class CurrencyHandler : ICurrencyHandler
{
    private IRatesBuilder _modelBuilder;
    private IRatesGetter _currencyRecipient;
    private IRabbitMQProducer _rabbitMQProducer;
    private CurrencyRates _result;
    private readonly ILogger<CurrencyHandler> _logger;

    public CurrencyHandler(IRatesBuilder modelbuilder,
        IRatesGetter currencyRecipient,
        ILogger<CurrencyHandler> logger,
        IRabbitMQProducer rabbitMQProducer)
    {
        _modelBuilder = modelbuilder;
        _currencyRecipient = currencyRecipient;
        _result = new CurrencyRates();
        _logger = logger;
        _rabbitMQProducer = rabbitMQProducer;
    }

    public async Task HandleAsync(object? sender, ElapsedEventArgs e)
    {
        try
        {
            _logger.LogInformation("Try handle primary api's response");

            var passedCurrencyPairs = await Policy.Handle<Exception>()
              .Retry(3, (e, i) => _logger.LogInformation(e.Message))
              .Execute(_currencyRecipient.GetCurrencyPairFromPrimary);

            _result.Rates = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs).Quotes;

        }
        catch (Exception ex)
        {
            if (ex is RatesBuildException || ex is HttpRequestException)
            {
                _logger.LogInformation("Try handle secondary api's response");

                var passedCurrencyPairs = await Policy.Handle<Exception>()
              .Retry(3, (e, i) => _logger.LogInformation(e.Message))
              .Execute(_currencyRecipient.GetCurrencyPairFromSecondary);

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
