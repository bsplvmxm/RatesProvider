namespace RatesProvider.Handler.Interfaces;

public interface IRatesBuilder
{
    T BuildPair<T>(string jsonString);

    public Dictionary<string, decimal> ConvertToDecimal(Dictionary<string, string> rates);
}