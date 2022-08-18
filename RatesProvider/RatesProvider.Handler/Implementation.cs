using RatesProvider.Handler.Interfaces;
using Timer = System.Timers.Timer;

namespace RatesProvider.Handler;
public class Implementation : IImplementation
{
    ICurrencyHandle _currencyHandle;

    public Implementation(ICurrencyHandle currencyHandle)
    {
        _currencyHandle = currencyHandle;
    }

    public void Run()
    {
        var period = 3600000;

        //Handle
        var timer = new Timer(period);

        timer.Elapsed += _currencyHandle.Handle;
        timer.AutoReset = true;
        timer.Enabled = true;
    }
}
