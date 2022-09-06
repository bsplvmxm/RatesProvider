namespace RatesProvider.Recipient.Infrastructure;

public static class EnvironmentVirable
{
    public static string PrimaryApiKey = "RATES_PROVIDER_PRIMARY_API_KEY";
    public static string SecondaryApiKey = "RATES_PROVIDER_SECONDARY_API_KEY";
    public static string Period = "RATES_PROVIDER_PERIOD";
    public static string BaseDirectory = "GOVNO_ZHOPA_BASE_DIRECTORY";
    public static string RabbitServer = "RABBIT_SERVER";
    public static string RabbitLogin = "RABBIT_SERVER_LOGIN";
    public static string RabbitPassword = "RABBIT_SERVER_PASSWORD";
}
