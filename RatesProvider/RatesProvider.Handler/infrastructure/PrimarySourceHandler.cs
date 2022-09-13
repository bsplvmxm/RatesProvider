using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Polly;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class PrimarySourceHandler : IRatesSourceHandler
{
    private readonly PrimaryRatesGetter _currencyRecipient;
    private readonly IHandleChecker _handleChecker;
    private readonly ILogger _logger;

    public PrimarySourceHandler(ILogger logger,
        ISettingsProvider settingsProvider,
        IRatesBuilder ratesBuilder,
        ISyncPolicy retryPolicy)
    {
        _logger = logger;
        _currencyRecipient = new PrimaryRatesGetter(settingsProvider, _logger);
        _handleChecker = new PrimaryHandleChecker(_logger, ratesBuilder, retryPolicy);
    }

    public async Task<NewRatesEvent> Handle() 
    {
        _logger.LogInformation("Try Handle primary RatesGetter");
        return await _handleChecker.Check(_currencyRecipient);
    }
}
