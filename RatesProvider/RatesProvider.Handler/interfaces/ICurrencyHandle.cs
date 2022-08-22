using System.Timers;

namespace RatesProvider.Handler.Interfaces;

public interface ICurrencyHandle
{
    Task HandleAsync(object? sender, ElapsedEventArgs e);
}
