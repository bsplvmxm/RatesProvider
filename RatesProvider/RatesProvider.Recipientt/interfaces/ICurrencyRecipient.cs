namespace RatesProvider.Recipient.interfaces;

public interface ICurrencyRecipient
{
    Task<string> GetCurrencyPairFromPrimary(string neededCurrency);
    string GetCurrencyPairFromSecondary(string neededCurrency);
}
