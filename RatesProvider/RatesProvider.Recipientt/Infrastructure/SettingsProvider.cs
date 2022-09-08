using RatesProvider.RatesGetter.Interfaces;
using IncredibleBackendContracts.Enums;

namespace RatesProvider.RatesGetter.Infrastructure;

public class SettingsProvider : ISettingsProvider
{
    public const string BaseCurrensy = "USD";

    public string GetEnvironmentVirableValue(string envName) =>
        Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.Machine)!;

    public string GetBaseCurrency() => BaseCurrensy;

    public string GetNeededCurrencies(bool isNeedToAddBase)
    {
        var rates = Enum.GetNames(typeof(TradingCurrency));

        if (isNeedToAddBase)
        {
            for (int i = 0; i < rates.Length; i++)
            {
                rates[i] = $"{BaseCurrensy}{rates[i]}";
            }
        }

        return string.Join(',', rates);

    }
}
