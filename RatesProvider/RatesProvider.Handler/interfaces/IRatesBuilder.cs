namespace RatesProvider.Handler.Interfaces;

public interface IRatesBuilder
{
    T BuildPair<T>(string jsonString);
}