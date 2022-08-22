using RatesProvider.Handler.infrastructure;
using RatesProvider.Handler.interfaces;
using RatesProvider.Handler.Models;
using System.Text.Json;

namespace RatesProvider.Handler;

public class ModelBuilder : IModelBuilder
{
    public string ErrorMessage { get; set; }
    public T BuildPair<T>(string jsonString)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var rates = JsonSerializer.Deserialize<T>(jsonString, options);
        ErrorModel? error;

        if (rates is null)
        {
            error = JsonSerializer.Deserialize<ErrorModel>(jsonString, options);
            ErrorMessage = $"statusCode: {error!.Code} \n info: {error.Info}";
            throw new ResponseException(ErrorMessage);
        }

        return rates;
    }
}