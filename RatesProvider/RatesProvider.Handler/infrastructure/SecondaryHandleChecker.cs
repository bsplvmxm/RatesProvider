﻿using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class SecondaryHandleChecker : IHandleChecker
{
    private readonly ILogger<SecondaryHandleChecker> _logger;
    private readonly IRatesBuilder _ratesBuilder;
    private readonly RetryPolicy _retryPolicy;
    private CurrencyRates _result;

    public SecondaryHandleChecker(ILogger<SecondaryHandleChecker> logger, IRatesBuilder ratesBuilder, RetryPolicy retryPolicy)
    {
        _logger = logger;
        _ratesBuilder = ratesBuilder;
        _result = new CurrencyRates();
        _retryPolicy = retryPolicy;
    }

    /// <summary>
    /// Check the posibility to handle passed Rates and return main model of Rates
    /// </summary>
    public async Task<CurrencyRates> Check<T>(T ratesGetter) where T : IRatesGetter
    {
        try
        {
            var passedRates = await _retryPolicy.Execute(ratesGetter.GetRates);
            _result.Rates = _ratesBuilder.ConvertToDecimal(_ratesBuilder.BuildPair<SecondaryRates>(passedRates).Data);
            return _result;
        }
        catch (Exception ex)
        {
            if (ex is RatesBuildException || ex is HttpRequestException)
            {
                _logger.LogInformation("failed: {0}", ex.Message);
            }
            else
            {
                _logger.LogInformation("failed: {0}", ex.Message);
            }
            return _result;
        }
    }
}