using RatesProvider.Recipient.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Recipient;

public class RatesGetter : IRatesGetter
{
    private readonly HttpClient _httpClient;
    private readonly string  _primaryApiKey;
    private readonly string _secondaryApiKey;
    private ISettingsProvider _settingsProvider;

    public RatesGetter(ISettingsProvider settingsProvider)
    {
        _httpClient = new HttpClient();
        _settingsProvider = settingsProvider;
        _primaryApiKey = _settingsProvider!.GetEnvironmentVirableValue(EnvironmentVirable.PrimaryApiKey);
        _secondaryApiKey = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.SecondaryApiKey);
        _httpClient.DefaultRequestHeaders.Add(Constant.HeaderKeyWord, _primaryApiKey);
    }

    public async Task<string> GetCurrencyPairFromPrimary()
    {
        var stringCurrency = _httpClient
            .GetStringAsync($"{Constant.PrimaryApiLink}{_settingsProvider.GetBaseCurrency()}{Constant.PrimaryApiParamCurrencies}{_settingsProvider.GetNeededCurrencies(false)}");
        var currency = await stringCurrency;

        return currency;
    }

    public async Task<string> GetCurrencyPairFromSecondary()
    {
        var stringCurrency = _httpClient
            .GetStringAsync($"{Constant.SecondaryApiLink}{_settingsProvider.GetNeededCurrencies(true)}{Constant.SecondaryApiParamKey}{_secondaryApiKey}");
        var currency = await stringCurrency;

        return currency;
    }
}
