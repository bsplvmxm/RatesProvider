namespace RatesProvider.Handler.Interfaces;

public interface IModelBuilder
{
    T BuildPair<T>(string jsonString);
}