using IncredibleBackendContracts.Events;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Interfaces;

public interface IHandleChecker
{
    Task<NewRatesEvent> Check<T>(T ratesGetter) where T : IRatesGetter;
}
