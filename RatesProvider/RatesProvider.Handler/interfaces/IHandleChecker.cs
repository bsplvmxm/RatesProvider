using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Interfaces;

public interface IHandleChecker
{
    Task<CurrencyRates> Check<T>(T ratesGetter) where T : IRatesGetter;
}
