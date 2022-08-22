using RatesProvider.Handler.infrastructure;
using RatesProvider.Handler.interfaces;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.interfaces;
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
            try
            {
                var passedCurrencyPairs = await _currencyRecipient.GetCurrencyPairFromPrimary("qwe");
                _result = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs);
                Console.WriteLine(((PrimaryRates)_result).Quotes["USDRUB"]);
            }
            catch (ResponseException)
            {
                var passedCurrencyPairs = _currencyRecipient.GetCurrencyPairFromSecondary("qwe");
                _result = _modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs);
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
