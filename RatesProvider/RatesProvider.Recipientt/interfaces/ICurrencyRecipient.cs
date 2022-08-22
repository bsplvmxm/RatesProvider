using RatesProvider.Recipient.Enums;

namespace RatesProvider.Recipient.Interfaces;
public interface ICurrencyRecipient
{
    Task<string> GetCurrencyPairFromPrimary(Rates source);
    Task<string> GetCurrencyPairFromSecondary(Rates source);
}
