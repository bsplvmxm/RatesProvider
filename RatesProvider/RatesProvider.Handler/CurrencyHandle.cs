using RatesProvider.Handler.infrastructure;
using RatesProvider.Handler.interfaces;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Interfaces;
using System.Timers;

namespace RatesProvider.Handler
{
    public class CurrencyHandle : ICurrencyHandle
    {
        private IModelBuilder _modelBuilder;
        private ICurrencyRecipient _currencyRecipient;
        private AbstractRates _result;

        public CurrencyHandle(IModelBuilder modelbuilder, ICurrencyRecipient currencyRecipient)
        {
            _modelBuilder = modelbuilder;
            _currencyRecipient = currencyRecipient;
        }

        public async Task HandleAsync(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Go\n");
            try
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromPrimary(Recipient.Enums.Rates.RUB);
                _result = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs);
                Console.WriteLine(((PrimaryRates)_result).Quotes["USDRUB"]);
            }
            catch (ResponseException)
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
