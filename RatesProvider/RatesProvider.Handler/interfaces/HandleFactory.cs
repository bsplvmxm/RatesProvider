using IncredibleBackendContracts.ExchangeModels;

namespace RatesProvider.Handler.Interfaces;

public interface HandleFactory
{
    Task<CurrencyRate> Handle();
}
