using RatesProvider.Recipient.interfaces;

namespace RatesProvider.Recipient;

public class CurrencyRecipient : ICurrencyRecipient
{
    public string GetCurrencyPairFromPrimary(string neededCurrency)
    {
        throw new NotImplementedException();
    }

    public string GetCurrencyPairFromSecondary(string neededCurrency)
    {
        throw new NotImplementedException();
    }

    public string GetNeededCurruncy()
    {
        throw new NotImplementedException();
    }
}
