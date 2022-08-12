namespace RatesProvider.Handler.Models
{
    public class SecondaryRates : AbstractRates
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
