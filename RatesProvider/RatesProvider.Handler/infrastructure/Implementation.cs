﻿using RatesProvider.Handler.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RatesProvider.Handler;
public class Implementation : IImplementation
{
    ICurrencyHandler _currencyHandle;

    public Implementation(ICurrencyHandler currencyHandle)
    {
        _currencyHandle = currencyHandle;
    }

    public async Task Run()
    {

        var period = Convert.ToInt32(Environment.GetEnvironmentVariable(EnvironmentVirable.Period, EnvironmentVariableTarget.Machine));

        //Handle
        var timer = new Timer(period);

        await _currencyHandle.HandleAsync(this, EventArgs.Empty as ElapsedEventArgs);

        timer.Elapsed += async (s, e) => await _currencyHandle.HandleAsync(s, e);
        timer.AutoReset = true;
        timer.Start();
        
    }
}
