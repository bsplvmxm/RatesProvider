using Microsoft.Extensions.Logging;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using IncredibleBackendContracts.ExchangeModels;

namespace RatesProvider.Handler.Infrastructure;

public class SecondaryHandler : HandleFactory
{
    private readonly SecondaryRatesGetter _currencyRecipient;
    private readonly SecondaryHandleChecker _handleChecker;

    public SecondaryHandler(ILogger logger,
        ISettingsProvider settingsProvider,
        IRatesBuilder ratesBuilder,
        RetryPolicy retryPolicy)
    {
        _currencyRecipient = new SecondaryRatesGetter(settingsProvider, logger);
        _handleChecker = new SecondaryHandleChecker(logger, ratesBuilder, retryPolicy);
    }

    public async Task<CurrencyRate> Handle()
    {
        return await _handleChecker.Check(_currencyRecipient);
    }
}
