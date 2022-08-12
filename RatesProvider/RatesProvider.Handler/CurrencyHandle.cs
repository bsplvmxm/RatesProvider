using RatesProvider.Handler.infrastructure;
using RatesProvider.Handler.interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.interfaces;

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

        public void Handle(object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

            var neededCurrencies = _currencyRecipient.GetNeededCurruncy();

            try
            {
                var passedCurrencyPairs = _currencyRecipient.GetCurrencyPairFromPrimary(neededCurrencies);
                _result = _modelBuilder.BuildPair<PrimaryRates>(passedCurrencyPairs);
            }
            catch (ResponseException)
            {
                var passedCurrencyPairs = _currencyRecipient.GetCurrencyPairFromSecondary(neededCurrencies);
                _result = _modelBuilder.BuildPair<SecondaryRates>(passedCurrencyPairs);
            }
            //finally
            //{
            //    throw new Exception(_modelBuilder.ErrorMessage);
            //}

            autoEvent.Set();
        }

    }
}
