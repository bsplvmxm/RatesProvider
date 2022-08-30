namespace RatesProvider.RatesGetter.Interfaces;

public interface ISettingsProvider
{
    public string GetEnvironmentVirableValue(string envName);
    public string GetBaseCurrency();
    public string GetNeededCurrencies(bool isNeedToAddBase);
}
