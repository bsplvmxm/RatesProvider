using RatesProvider.Recipient.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using Microsoft.Extensions.Logging;

namespace RatesProvider.Recipient;

public class RatesGetter : IRatesGetter
{
    private readonly HttpClient _httpClient;
    private readonly string  _primaryApiKey;
    private readonly string _secondaryApiKey;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger<RatesGetter> _logger;

    public RatesGetter(ISettingsProvider settingsProvider, ILogger<RatesGetter> logger)
    {
        _httpClient = new HttpClient();
        _settingsProvider = settingsProvider;
        _primaryApiKey = _settingsProvider!.GetEnvironmentVirableValue(EnvironmentVirable.PrimaryApiKey);
        _secondaryApiKey = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.SecondaryApiKey);
        _httpClient.DefaultRequestHeaders.Add(Constant.HeaderKeyWord, _primaryApiKey);
        _logger = logger;
    }

    public async Task<string> GetCurrencyPairFromPrimary()
    {
        _logger.LogInformation("Send request to primary API");

        var stringCurrency = _httpClient
            .GetStringAsync($"{Constant.PrimaryApiLink}{_settingsProvider.GetBaseCurrency()}{Constant.PrimaryApiParamCurrencies}{_settingsProvider.GetNeededCurrencies(false)}");

        return await stringCurrency;
    }

    public async Task<string> GetCurrencyPairFromSecondary()
    {
        _logger.LogInformation("Send request to secondary API");

        var stringCurrency = _httpClient
            .GetStringAsync($"{Constant.SecondaryApiLink}{_settingsProvider.GetNeededCurrencies(true)}{Constant.SecondaryApiParamKey}{_secondaryApiKey}");

        return await stringCurrency;
    }
}
