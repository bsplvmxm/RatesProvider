namespace RatesProvider.Recipient;

public class CurrencyPair
{
    public string BaseCurrensy { get; set; }
    public Dictionary<string, decimal> QuotedCurrency { get; set; }
}
