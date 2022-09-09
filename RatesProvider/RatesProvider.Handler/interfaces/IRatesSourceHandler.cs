using IncredibleBackendContracts.ExchangeModels;

namespace RatesProvider.Handler.Interfaces;

public interface IRatesSourceHandler
{
    Task<CurrencyRate> Handle();
}
