using RatesProvider.Handler.interfaces;
using RatesProvider.Handler.Models;
using System.Text.Json;

namespace RatesProvider.Handler;

public class ModelBuilder : IModelBuilder
{
    public T BuildPair<T>(string jsonString)
    {
        var rates = JsonSerializer.Deserialize<T>(jsonString);
        ErrorModel? error;

        if (rates is null)
        {
            error = JsonSerializer.Deserialize<ErrorModel>(jsonString);
            throw new Exception($"statusCode: {error!.Code} \n info: {error.Info}");
        }

        return rates;
    }
}