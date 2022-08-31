using RatesProvider.Handler.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using System.Timers;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler;
public class Implementation : IImplementation
{
    ISettingsProvider _settingsProvider;
    ICurrencyHandler _currencyHandle;
    ILogger _logger;

    public Implementation(ICurrencyHandler currencyHandle, ILogger logger, ISettingsProvider settingsProvider)
    {
        _currencyHandle = currencyHandle;
        _logger = logger;
        _settingsProvider = settingsProvider;
    }

    public async Task Run()
    {
        var period = Convert.ToInt32(_settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.Period));
        _logger.LogInformation("Start implementation with period {0}", period);

        //Handle
        var timer = new Timer(period);

        await _currencyHandle.HandleAsync(this, EventArgs.Empty as ElapsedEventArgs);

        timer.Elapsed += async (s, e) => await _currencyHandle.HandleAsync(s, e);
        timer.AutoReset = true;
        timer.Start();
        
    }
}
