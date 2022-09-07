using IncredibleBackendContracts.ExchangeModels;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Interfaces;

public interface IHandleChecker
{
    Task<CurrencyRate> Check<T>(T ratesGetter) where T : IRatesGetter;
}
