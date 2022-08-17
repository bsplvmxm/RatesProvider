using Timer = System.Timers.Timer;

namespace RatesProvider.Handler;
public class Implementation
{
    public static void Run(CurrencyHandle currencyHandle)
    {
        var period = 3600000;

        //Handle
        var timer = new Timer(period);

        timer.Elapsed += currencyHandle.Handle;
        timer.AutoReset = true;
        timer.Enabled = true;
    }
}
