namespace RatesProvider.RatesGetter.Infrastructure;

public static class Constant
{
    public const string HeaderKeyWord = "apikey";
    public const string PrimaryApiLink = "https://api.apilayer.com/currency_data/live?source=";
    public const string SecondaryApiLink = "https://currate.ru/api/?get=rates&pairs=";
    public const string PrimaryApiParamCurrencies = "&currencies=";
    public const string SecondaryApiParamKey = "&key=";
    public const string SettingsDirectory = @"C:\Users\govnoZhopa\Source\Repos\RatesProvider\RatesProvider\RatesProvider.Handler";
    public const string QueueName = "rates";
    public const string RootKey = "rates";
    public const string Host = "localhost";
    public const string Exchange = "";
    public const int CountRetry = 3;
}
