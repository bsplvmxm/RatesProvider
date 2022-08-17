using RatesProvider.Handler;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient;
using System.Timers;

//setup
var modelBuilder = new ModelBuilder();
var currencyRecipient = new CurrencyRecipient();
var currancyHandle = new CurrencyHandle(modelBuilder, currencyRecipient);

var autoEvent = new AutoResetEvent(false);
var dueTime = 86400000;
var period = 3600000;

//Handle
timer = new Timer(period);

timer.Elapsed += currancyHandle.Handle();
timer.AutoReset = true;
timer.Enabled = true;