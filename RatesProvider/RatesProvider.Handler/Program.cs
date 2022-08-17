using RatesProvider.Handler;
using RatesProvider.Recipient;

//setup
var modelBuilder = new ModelBuilder();
var currencyRecipient = new CurrencyRecipient();
var currencyHandle = new CurrencyHandle(modelBuilder, currencyRecipient);

Implementation.Run(currencyHandle);