using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace RatesProvider.Handler
{
    public class CurrencyHandler : ICurrencyHandler
    {
        private IRatesBuilder _modelBuilder;
        private IRatesGetter _currencyRecipient;
        private CurrencyRates _result;
        private readonly ILogger _logger;

        public CurrencyHandler(IRatesBuilder modelbuilder, IRatesGetter currencyRecipient, ILogger logger)
        {
            _modelBuilder = modelbuilder;
            _currencyRecipient = currencyRecipient;
            _result = new CurrencyRates();
            _logger = logger;
        }

        public async Task HandleAsync(object? sender, ElapsedEventArgs e)
        {
            try
            {
                // retry policy must be applied for both primary and secondary sources
                _logger.LogInformation("Try handle primary api's response");
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromPrimary();
                _result.Rates = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs).Quotes;
            }
            catch (RatesBuildException)
            {
                _logger.LogInformation("Try handle secondary api's response");
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary();
                _result.Rates = _modelBuilder.ConvertToDecimal(_modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs).Data);
            }
            catch (HttpRequestException)
            {
                _logger.LogInformation("Try handle secondary api's response");
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary();
                _result.Rates = _modelBuilder.ConvertToDecimal(_modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs).Data);
            }
            catch (Exception msg)
            {
                _logger.LogInformation("Unprocessable response: {0}", msg);
            }
        }
    }
}
