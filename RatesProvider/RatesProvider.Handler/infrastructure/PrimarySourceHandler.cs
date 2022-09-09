using Microsoft.Extensions.Logging;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using IncredibleBackendContracts.ExchangeModels;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class PrimarySourceHandler : IRatesSourceHandler
{
    private readonly PrimaryRatesGetter _currencyRecipient;
    private readonly PrimaryHandleChecker _handleChecker;
    private readonly ILogger _logger;

    public PrimarySourceHandler(ILogger logger,
        ISettingsProvider settingsProvider,
        IRatesBuilder ratesBuilder,
        RetryPolicy retryPolicy)
    {
        _logger = logger;
        _currencyRecipient = new PrimaryRatesGetter(settingsProvider, _logger);
        _handleChecker = new PrimaryHandleChecker(_logger, ratesBuilder, retryPolicy);
    }

    public async Task<CurrencyRate> Handle() 
    {
        _logger.LogInformation("Try Handle primary RatesGetter");
        return await _handleChecker.Check(_currencyRecipient);
    }
}
