using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;
using System.Timers;

namespace RatesProvider.Handler
{
    public class CurrencyHandler : ICurrencyHandler
    {
        private IRatesBuilder _modelBuilder;
        private IRatesGetter _currencyRecipient;
        private CurrencyRates _result;

        public CurrencyHandler(IRatesBuilder modelbuilder, IRatesGetter currencyRecipient)
        {
            _modelBuilder = modelbuilder;
            _currencyRecipient = currencyRecipient;
            _result = new CurrencyRates();
        }

        public async Task HandleAsync(object? sender, ElapsedEventArgs e)
        {
            try
            {
                // retry policy must be applied for both primary and secondary sources
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromPrimary();
                _result.Rates = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs).Quotes;
            }
            catch (RatesBuildException)
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary();
                _result.Rates = _modelBuilder.ConvertToDecimal(_modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs).Data);
            }
            catch (HttpRequestException)
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary();
                _result.Rates = _modelBuilder.ConvertToDecimal(_modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs).Data);
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
