using RatesProvider.Handler.interfaces;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Infrastructure;
using System.Text.Json;

namespace RatesProvider.Handler;

public class ModelBuilder : IModelBuilder
{
    public T BuildPair<T>(string jsonString)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var rates = JsonSerializer.Deserialize<T>(jsonString, options);

        if (rates is null)
            throw new BuildException(ErrorMessage.BuildException);

        return rates;
    }
}