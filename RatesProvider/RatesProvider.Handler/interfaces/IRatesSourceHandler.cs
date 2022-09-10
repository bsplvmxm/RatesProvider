using IncredibleBackendContracts.Events;

namespace RatesProvider.Handler.Interfaces;

public interface IRatesSourceHandler
{
    Task<NewRatesEvent> Handle();
}
