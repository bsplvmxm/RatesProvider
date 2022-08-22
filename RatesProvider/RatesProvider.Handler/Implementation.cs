using RatesProvider.Handler.Interfaces;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RatesProvider.Handler;
public class Implementation : IImplementation
{
    ICurrencyHandle _currencyHandle;

    public Implementation(ICurrencyHandle currencyHandle)
    {
        _currencyHandle = currencyHandle;
    }

    public async Task Run()
    {

        var period = 15000;

        //Handle
        var timer = new Timer(period);

        await _currencyHandle.HandleAsync(this, EventArgs.Empty as ElapsedEventArgs);

        timer.Elapsed += async (s, e) => await _currencyHandle.HandleAsync(s, e);
        timer.AutoReset = true;
        timer.Start();
        
    }
}
