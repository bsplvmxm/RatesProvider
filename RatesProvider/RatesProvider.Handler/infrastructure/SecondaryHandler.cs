using Microsoft.Extensions.Logging;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class SecondaryHandler : HandleFactory
{
    private readonly SecondaryRatesGetter _currencyRecipient;
    private readonly SecondaryHandleChecker _handleChecker;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger _logger;
    private readonly IRatesBuilder _modelBuilder;
    public SecondaryHandler(ILogger logger,
        ISettingsProvider settingsProvider,
        IRatesBuilder ratesBuilder,
        RetryPolicy retryPolicy)
    {
        _logger = logger;
        _settingsProvider = settingsProvider;
        _modelBuilder = ratesBuilder;
        _currencyRecipient = new SecondaryRatesGetter(_settingsProvider, _logger);
        _handleChecker = new SecondaryHandleChecker(_logger, _modelBuilder, retryPolicy);
    }
    public async Task<CurrencyRates> Handle()
    {
        return await _handleChecker.Check(_currencyRecipient);
    }
}
