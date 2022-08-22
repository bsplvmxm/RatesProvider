namespace RatesProvider.Handler.Models;

public class PrimaryRates : AbstractRates
{
    public Dictionary<string, decimal> Quotes { get; set; }
}
