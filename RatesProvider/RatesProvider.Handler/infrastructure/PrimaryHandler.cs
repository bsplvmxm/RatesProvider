using Microsoft.Extensions.Logging;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using IncredibleBackendContracts.ExchangeModels;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class PrimaryHandler : HandleFactory
{
    private readonly PrimaryRatesGetter _currencyRecipient;
    private readonly PrimaryHandleChecker _handleChecker;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger _logger;
    private readonly IRatesBuilder _modelBuilder;

    public PrimaryHandler(ILogger logger,
        ISettingsProvider settingsProvider,
        IRatesBuilder ratesBuilder,
        RetryPolicy retryPolicy)
    {
        _logger = logger;
        _settingsProvider = settingsProvider;
        _modelBuilder = ratesBuilder;
        _currencyRecipient = new PrimaryRatesGetter(_settingsProvider, _logger);
        _handleChecker = new PrimaryHandleChecker(_logger, _modelBuilder, retryPolicy);
    }
    public async Task<CurrencyRate> Handle() 
    {
        return await _handleChecker.Check(_currencyRecipient);
    }
}
