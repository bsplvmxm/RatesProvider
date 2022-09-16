using System.Timers;
using Microsoft.Extensions.Logging;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Infrastructure;
using IncredibleBackendContracts.Events;
using IncredibleBackend.Messaging.Interfaces;

namespace RatesProvider.Handler;

public class CurrencyHandler : ICurrencyHandler
{
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger _logger;
    private List<IRatesSourceHandler> _ratesSourceHandlers;
    private NewRatesEvent _result;

    public CurrencyHandler(IRatesBuilder ratesBuilder,
        ILogger<CurrencyHandler> logger,
        IMessageProducer messageProducer,
        ISettingsProvider settingsProvider,
        IRetryPolicySettings retryPolicySettings)
    {
        _messageProducer = messageProducer;
        _logger = logger;
        _result = new NewRatesEvent();
        _ratesSourceHandlers = new List<IRatesSourceHandler>()
        {
            new PrimarySourceHandler(_logger, settingsProvider, ratesBuilder, retryPolicySettings.BuildRetryPolicy()),
            new SecondarySourceHandler(_logger, settingsProvider, ratesBuilder, retryPolicySettings.BuildRetryPolicy())
        };
    }

    public async Task HandleAsync(object? sender, ElapsedEventArgs e)
    {
        foreach (var ratesSourceHandler in _ratesSourceHandlers)
        {
            _result = await ratesSourceHandler.Handle();
            _logger.LogInformation("Finish handle with {0} elements in dictionary", _result.Rates.Count);

            if(_result.Rates.Count != 0)
                break;
        }

        await _messageProducer.ProduceMessage(_result, "Send rates to queue");
    }
}