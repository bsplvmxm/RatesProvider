using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class SecondaryHandleChecker : IHandleChecker
{
    private readonly ILogger _logger;
    private readonly IRatesBuilder _ratesBuilder;
    private readonly ISyncPolicy _retryPolicy;
    private NewRatesEvent _result;

    public SecondaryHandleChecker(ILogger logger, IRatesBuilder ratesBuilder, ISyncPolicy retryPolicy)
    {
        _logger = logger;
        _ratesBuilder = ratesBuilder;
        _result = new NewRatesEvent();
        _retryPolicy = retryPolicy;
    }

    /// <summary>
    /// Check the posibility to handle passed Rates and return main model of Rates
    /// </summary>
    public async Task<NewRatesEvent> Check<T>(T ratesGetter) where T : IRatesGetter
    {
        try
        {
            var passedRates = await _retryPolicy.Execute(ratesGetter.GetRates);
            _result.Rates = _ratesBuilder.ConvertToDecimal(_ratesBuilder.BuildPair<SecondaryRates>(passedRates).Data);
            return _result;
        }
        catch (Exception ex)
        {
           _logger.LogError("failed: {0}", ex.Message);

            return _result;
        }
    }
}
