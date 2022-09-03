using Microsoft.Extensions.Logging;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.RatesGetter.Infrastructure;

public class PrimaryRatesGetter : IRatesGetter
{
    private readonly HttpClient _httpClient;
    private readonly string _primaryApiKey;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger<PrimaryRatesGetter> _logger;

    public PrimaryRatesGetter(ISettingsProvider settingsProvider, ILogger<PrimaryRatesGetter> logger)
    {
        _httpClient = new HttpClient();
        _settingsProvider = settingsProvider;
        _primaryApiKey = _settingsProvider!.GetEnvironmentVirableValue(EnvironmentVirable.PrimaryApiKey);
        _httpClient.DefaultRequestHeaders.Add(Constant.HeaderKeyWord, _primaryApiKey);
        _logger = logger;
    }

    public async Task<string> GetRates()
    {
        _logger.LogInformation("Send request to primary API");

        var stringCurrency = _httpClient
            .GetStringAsync($"{Constant.PrimaryApiLink}{_settingsProvider.GetBaseCurrency()}{Constant.PrimaryApiParamCurrencies}{_settingsProvider.GetNeededCurrencies(false)}");

        return await stringCurrency;
    }
}
