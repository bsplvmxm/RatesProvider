using RatesProvider.Handler.infrastructure;
using RatesProvider.Handler.interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.interfaces;
using System.Timers;

namespace RatesProvider.Handler
{
    public class CurrencyHandle
    {
        private IModelBuilder _modelBuilder;
        private ICurrencyRecipient _currencyRecipient;
        private AbstractRates _result;

        public CurrencyHandle(IModelBuilder modelbuilder, ICurrencyRecipient currencyRecipient)
        {
            _modelBuilder = modelbuilder;
            _currencyRecipient = currencyRecipient;
        }

        public void Handle(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var passedCurrencyPairs = _currencyRecipient.GetCurrencyPairFromPrimary("qwe");
                _result = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs);
            }
            catch (ResponseException)
            {
                var passedCurrencyPairs = _currencyRecipient.GetCurrencyPairFromSecondary("qwe");
                _result = _modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs);
            }
            catch
            {
                throw new Exception(_modelBuilder.ErrorMessage);
            }
        }
    }
}
