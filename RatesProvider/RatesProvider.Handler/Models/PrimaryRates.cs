namespace RatesProvider.Handler.Models;

public class PrimaryRates
{
    public Dictionary<string, decimal> Quotes { get; set; }

    public PrimaryRates()
    {
        Quotes = new Dictionary<string, decimal>();
    }
}
