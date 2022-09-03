using RatesProvider.Recipient.Enums;

namespace RatesProvider.Recipient.Interfaces;
public interface IRatesGetter
{
    Task<string> GetRates();
}
