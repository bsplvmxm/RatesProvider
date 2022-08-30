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
        }

        public async Task HandleAsync(object? sender, ElapsedEventArgs e)
        {
            try
            {
                // retry policy must be applied for both primary and secondary sources
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromPrimary();
                _result = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs);
            }
            catch (RatesBuildException)
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary();
                _result = _modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs);
            }
            catch (HttpRequestException)
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary();
                _result = _modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs);
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
