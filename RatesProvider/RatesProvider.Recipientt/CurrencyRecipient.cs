using RatesProvider.Recipient.Interfaces;
using RatesProvider.Recipient.Enums;
using System.Net.Http.Headers;

namespace RatesProvider.Recipient;

public class CurrencyRecipient : ICurrencyRecipient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public async Task<string> GetCurrencyPairFromPrimary(Rates source)
    {
        _httpClient.DefaultRequestHeaders.Add("apikey", "IQVeyd6CCjX7knaIZAHSWkEWH0VF6Dm8");

        var stringCurrency = _httpClient
            .GetStringAsync("https://api.apilayer.com/currency_data/live?source=USD&currencies=RUB,EUR,JPY,AMD,BGN,RSD");
        var currency = await stringCurrency;

        return currency;
    }

    public async Task<string> GetCurrencyPairFromSecondary(Rates source)
    {
        var stringCurrency = _httpClient
            .GetStringAsync("https://currate.ru/api/?get=rates&pairs=USDRUB,USDEUR,USDJPY,USDAMD,USDBGN,USDRSD&key=a9f81693d8196acd20378dba8ceff0db");
        var currency = await stringCurrency;

        return currency;
    }
}
