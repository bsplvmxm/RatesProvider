namespace RatesProvider.Handler.interfaces;

public interface IModelBuilder
{
    T BuildPair<T>(string jsonString);
}