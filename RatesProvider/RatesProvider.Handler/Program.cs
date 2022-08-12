using RatesProvider.Handler;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient;

//setup
var modelBuilder = new ModelBuilder();
var currencyRecipient = new CurrencyRecipient();
var currancyHandle = new CurrencyHandle(modelBuilder, currencyRecipient);

var autoEvent = new AutoResetEvent(false);
var dueTime = 86400000;
var period = 3600000;

//Handle
var timer = new Timer(currancyHandle.Handle, autoEvent, dueTime, period);