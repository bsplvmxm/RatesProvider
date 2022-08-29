namespace RatesProvider.Handler.Models;

public class PrimaryRates : CurrencyRates
{
    public Dictionary<string, decimal> Quotes { get; set; }
}
