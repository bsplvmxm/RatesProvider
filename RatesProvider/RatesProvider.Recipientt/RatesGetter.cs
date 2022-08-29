﻿using RatesProvider.Recipient.Interfaces;
using RatesProvider.Recipient.Enums;
using RatesProvider.Recipient.Infrastructure;

namespace RatesProvider.Recipient;

public class RatesGetter : IRatesGetter
{
    private readonly HttpClient _httpClient;
    private readonly string  _primaryApiKey;
    private readonly string _secondaryApiKey;

    public RatesGetter()
    {
        _httpClient = new HttpClient();
        // _primaryApiKey = SettingsProvider.GetSetting(EnvironmentVirable.PrimaryApiKey);
        _primaryApiKey = Environment.GetEnvironmentVariable(EnvironmentVirable.PrimaryApiKey, EnvironmentVariableTarget.Machine)!;
        _secondaryApiKey = Environment.GetEnvironmentVariable(EnvironmentVirable.SecondaryApiKey, EnvironmentVariableTarget.Machine)!;
        _httpClient.DefaultRequestHeaders.Add("apikey", _primaryApiKey);
    }

    public async Task<string> GetCurrencyPairFromPrimary(Rates source)
    {
        // "base currency" must be a setting
        var stringCurrency = _httpClient
            .GetStringAsync("https://api.apilayer.com/currency_data/live?source=USD&currencies=RUB,EUR,JPY,AMD,BGN,RSD");
        var currency = await stringCurrency;

        return currency;
    }

    public async Task<string> GetCurrencyPairFromSecondary(Rates source)
    {
        var stringCurrency = _httpClient
            .GetStringAsync($"https://currate.ru/api/?get=rates&pairs=USDRUB,USDEUR,USDJPY,USDAMD,USDBGN,USDRSD&key={_secondaryApiKey}");
        var currency = await stringCurrency;

        return currency;
    }
}