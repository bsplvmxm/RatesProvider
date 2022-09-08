using Microsoft.Extensions.Logging;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Recipient.Interfaces;
using IncredibleBackendContracts.ExchangeModels;
using RatesProvider.Handler.Models;

namespace RatesProvider.Handler.Infrastructure;

public class PrimaryHandleChecker : IHandleChecker
{
    private readonly ILogger _logger;
    private readonly IRatesBuilder _ratesBuilder;
    private readonly RetryPolicy _retryPolicy;
    private CurrencyRate _result;

    public PrimaryHandleChecker(ILogger logger, IRatesBuilder ratesBuilder, RetryPolicy retryPolicy)
    {
        _logger = logger;
        _ratesBuilder = ratesBuilder;
        _result = new CurrencyRate();
        _retryPolicy = retryPolicy;
    }

    /// <summary>
    /// Check the posibility to handle passed Rates and return main model of Rates
    /// </summary>
    public async Task<CurrencyRate> Check<T>(T ratesGetter) where T: IRatesGetter
    {
        try
        {
            _logger.LogInformation("try to build Rates");
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
