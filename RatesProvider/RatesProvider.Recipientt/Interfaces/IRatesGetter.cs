namespace RatesProvider.Recipient.Interfaces;
public interface IRatesGetter
{
    Task<string> GetRates();
}
