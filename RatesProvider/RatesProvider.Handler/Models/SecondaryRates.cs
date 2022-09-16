namespace RatesProvider.Handler.Models;

public class SecondaryRates
{
    public Dictionary<string, string> Data { get; set; }

    public SecondaryRates()
    {
        Data = new Dictionary<string, string>();
    }
}
