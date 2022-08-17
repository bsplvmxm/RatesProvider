namespace RatesProvider.Handler.interfaces;

public interface IModelBuilder
{
    string ErrorMessage { get; set; }
    T BuildPair<T>(string jsonString);
}