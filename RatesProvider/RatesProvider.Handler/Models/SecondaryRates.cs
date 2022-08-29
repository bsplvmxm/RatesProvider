namespace RatesProvider.Handler.Models;

public class SecondaryRates : CurrencyRates
{
    public Dictionary<string, string> Data { get; set; }
}
