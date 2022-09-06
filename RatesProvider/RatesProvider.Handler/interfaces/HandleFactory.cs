using RatesProvider.Handler.Models;

namespace RatesProvider.Handler.Interfaces;

public interface HandleFactory
{
    Task<CurrencyRates> Handle();
}
