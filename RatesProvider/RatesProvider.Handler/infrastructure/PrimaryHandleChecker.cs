﻿using Microsoft.Extensions.Logging;
using Polly;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class PrimaryHandleChecker : IHandleChecker
{
    private readonly ILogger<PrimaryHandleChecker> _logger;
    private readonly IRatesBuilder _ratesBuilder;
    private CurrencyRates _result;

    public PrimaryHandleChecker(ILogger<PrimaryHandleChecker> logger, IRatesBuilder ratesBuilder)
    {
        _logger = logger;
        _ratesBuilder = ratesBuilder;
        _result = new CurrencyRates();
    }

    /// <summary>
    /// Check the posibility to handle passed Rates and return main model of Rates
    /// </summary>
    public async Task<CurrencyRates> Check<T>(T ratesGetter) where T: IRatesGetter
    {
        try
        {
            var passedRates = await Policy.Handle<Exception>()
                  .Retry(3, (e, i) => _logger.LogInformation(e.Message))
                  .Execute(ratesGetter.GetRates);
            _result.Rates = _ratesBuilder.BuildPair<PrimaryRates>(passedRates).Quotes;
            return _result;
        } 
        catch(Exception ex)
        {
            if(ex is RatesBuildException || ex is HttpRequestException)
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
