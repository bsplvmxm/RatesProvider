using System.Timers;

namespace RatesProvider.Handler.Interfaces;

public interface ICurrencyHandle
{
    void Handle(object? sender, ElapsedEventArgs e);
}
