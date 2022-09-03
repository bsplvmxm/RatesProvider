using Microsoft.Extensions.Logging;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Infrastructure;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.RatesGetter.Infrastructure;

public class SecondaryRatesGetter : IRatesGetter
{
    private readonly HttpClient _httpClient;
    private readonly string _secondaryApiKey;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger<SecondaryRatesGetter> _logger;

    public SecondaryRatesGetter(ISettingsProvider settingsProvider, ILogger<SecondaryRatesGetter> logger)
    {
        _httpClient = new HttpClient();
        _settingsProvider = settingsProvider;
        _secondaryApiKey = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.SecondaryApiKey);
        _logger = logger;
    }

    public async Task<string> GetRates()
    {
        _logger.LogInformation("Send request to secondary API");

        var stringCurrency = _httpClient
            .GetStringAsync($"{Constant.SecondaryApiLink}{_settingsProvider.GetNeededCurrencies(true)}{Constant.SecondaryApiParamKey}{_secondaryApiKey}");

        return await stringCurrency;
    }
}
