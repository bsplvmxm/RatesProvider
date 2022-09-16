using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Polly;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler.Infrastructure;

public class SecondarySourceHandler : IRatesSourceHandler
{
    private readonly SecondaryRatesGetter _currencyRecipient;
    private readonly SecondaryHandleChecker _handleChecker;
    private readonly ILogger _logger;

    public SecondarySourceHandler(ILogger logger,
        ISettingsProvider settingsProvider,
        IRatesBuilder ratesBuilder,
        ISyncPolicy retryPolicy)
    {
        _currencyRecipient = new SecondaryRatesGetter(settingsProvider, logger);
        _handleChecker = new SecondaryHandleChecker(logger, ratesBuilder, retryPolicy);
        _logger = logger;
    }

    public async Task<NewRatesEvent> Handle()
    {
        _logger.LogInformation("handle primary RatesGetter ends with 0 elements in Dictionary, Try Handle secondary RatesGetter");
        return await _handleChecker.Check(_currencyRecipient);
    }
}
