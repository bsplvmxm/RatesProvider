using IncredibleBackendContracts.Models;

namespace RatesProvider.Handler.Interfaces;

public interface HandleFactory
{
    Task<CurrencyRate> Handle();
}
