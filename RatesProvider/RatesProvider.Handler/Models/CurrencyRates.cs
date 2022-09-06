namespace RatesProvider.Handler.Models;

public  class CurrencyRates
{
    public Dictionary<string, decimal> Rates;

    public CurrencyRates()
    {
        Rates = new Dictionary<string, decimal>();
    }
}
