using RatesProvider.Recipient.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using RatesProvider.RatesGetter.Infrastructure;


namespace RatesProvider.Recipient;

public class RatesGetter : IRatesGetter
{
    public Task<string> GetRates()
    {
        throw new NotImplementedException();
    }
}
