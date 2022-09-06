using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class PrimaryHandleChecker : IHandleChecker
{
    private readonly ILogger _logger;
    private readonly IRatesBuilder _ratesBuilder;
    private readonly RetryPolicy _retryPolicy;
    private CurrencyRates _result;

    public PrimaryHandleChecker(ILogger logger, IRatesBuilder ratesBuilder, RetryPolicy retryPolicy)
    {
        _logger = logger;
        _ratesBuilder = ratesBuilder;
        _result = new CurrencyRates();
        _retryPolicy = retryPolicy;
    }

    /// <summary>
    /// Check the posibility to handle passed Rates and return main model of Rates
    /// </summary>
    public async Task<CurrencyRates> Check<T>(T ratesGetter) where T: IRatesGetter
    {
        try
        {
            //log
            var passedRates = await _retryPolicy.Execute(ratesGetter.GetRates);
            _result.Rates = _ratesBuilder.BuildPair<PrimaryRates>(passedRates).Quotes;
            return _result;
        } 
        catch(Exception ex)
        {
            _logger.LogError("failed: {0}", ex.Message);
            return _result;
        }
    }
}
