using RatesProvider.Handler.Interfaces;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Infrastructure;
using System.Globalization;
using System.Text.Json;

namespace RatesProvider.Handler;

public class RatesBuilder : IRatesBuilder
{
    public T BuildPair<T>(string jsonString)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var rates = JsonSerializer.Deserialize<T>(jsonString, options);

        if (rates is null)
            throw new RatesBuildException(ErrorMessage.RatesBuildException);

        return rates;
    }

    public Dictionary<string, decimal> ConvertToDecimal(Dictionary<string,string> rates)
    {
        var result = new Dictionary<string, decimal>();
        decimal temp;
        foreach (var rate in rates)
        {
            temp = decimal.Parse(rate.Value, CultureInfo.InvariantCulture);
            result.Add(rate.Key, temp);
        }

        return result;
    }
}