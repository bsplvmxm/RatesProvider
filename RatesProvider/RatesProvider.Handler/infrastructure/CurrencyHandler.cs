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
            Console.WriteLine("Go\n");
            try
            {
                // retry policy must be applied for both primary and secondary sources
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromPrimary(Recipient.Enums.Rates.RUB);
                _result = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs);
                Console.WriteLine(((PrimaryRates)_result).Quotes["USDRUB"]);
            }
            catch (RatesBuildException)
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary(Recipient.Enums.Rates.RUB);
                _result = _modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs);
                Console.WriteLine(((SecondaryRates)_result).Data["USDRUB"]);
            }
            catch (HttpRequestException)
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromSecondary(Recipient.Enums.Rates.RUB);
                _result = _modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs);
                Console.WriteLine(((SecondaryRates)_result).Data["USDRUB"]);
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
