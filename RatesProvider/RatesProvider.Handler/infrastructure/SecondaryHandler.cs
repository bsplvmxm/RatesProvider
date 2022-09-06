using Microsoft.Extensions.Logging;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using IncredibleBackendContracts.Models;

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
    public async Task<CurrencyRate> Handle()
    {
        return await _handleChecker.Check(_currencyRecipient);
    }
}
