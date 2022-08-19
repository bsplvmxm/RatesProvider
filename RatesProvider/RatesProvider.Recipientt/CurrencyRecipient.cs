using RatesProvider.Recipient.interfaces;
using System.Net.Http.Headers;

namespace RatesProvider.Recipient;

public class CurrencyRecipient : ICurrencyRecipient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public async Task<string> GetCurrencyPairFromPrimary(string neededCurrency)
    {
        _httpClient.DefaultRequestHeaders.Add("apikey", "mbuKZX6GWcumRQ7KGgsw0FWAQ4IRiTmR");

        var stringCurrency = _httpClient.GetStringAsync("https://api.apilayer.com/currency_data/live?source=USD&currencies=RUB,EUR,JPY,AMD,BGN,RSD");
        var currency = await stringCurrency;

        return currency;
    }

    public string GetCurrencyPairFromSecondary(string neededCurrency)
    {
        throw new NotImplementedException();
    }
}
