using Microsoft.Extensions.Logging;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using IncredibleBackendContracts.ExchangeModels;

namespace RatesProvider.Handler.Infrastructure;

public class SecondarySourceHandler : IRatesSourceHandler
{
    private readonly SecondaryRatesGetter _currencyRecipient;
    private readonly SecondaryHandleChecker _handleChecker;
    private readonly ILogger _logger;

    public SecondarySourceHandler(ILogger logger,
        ISettingsProvider settingsProvider,
        IRatesBuilder ratesBuilder,
        RetryPolicy retryPolicy)
    {
        _currencyRecipient = new SecondaryRatesGetter(settingsProvider, logger);
        _handleChecker = new SecondaryHandleChecker(logger, ratesBuilder, retryPolicy);
        _logger = logger;
    }

    public async Task<CurrencyRate> Handle()
    {
        _logger.LogInformation("handle primary RatesGetter ends with 0 elements in Dictionary, Try Handle secondary RatesGetter");
        return await _handleChecker.Check(_currencyRecipient);
    }
}
