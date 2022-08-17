using RatesProvider.Handler;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient;
using Timer = System.Timers.Timer;

//setup
var modelBuilder = new ModelBuilder();
var currencyRecipient = new CurrencyRecipient();
var currencyHandle = new CurrencyHandle(modelBuilder, currencyRecipient);

var period = 3600000;

//Handle
var timer = new Timer(period);

timer.Elapsed += currencyHandle.Handle();
timer.AutoReset = true;
timer.Enabled = true;