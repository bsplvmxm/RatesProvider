using System.Timers;

namespace RatesProvider.Handler.Interfaces;

public interface ICurrencyHandler
{
    Task HandleAsync(object? sender, ElapsedEventArgs e);
}
