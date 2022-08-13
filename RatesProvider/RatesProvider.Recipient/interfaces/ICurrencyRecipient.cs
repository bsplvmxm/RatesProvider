namespace RatesProvider.Recipient.interfaces;

public interface ICurrencyRecipient
{
    string GetCurrencyPairFromPrimary(string neededCurrency);
    string GetCurrencyPairFromSecondary(string neededCurrency);
    string GetNeededCurruncy();
}
