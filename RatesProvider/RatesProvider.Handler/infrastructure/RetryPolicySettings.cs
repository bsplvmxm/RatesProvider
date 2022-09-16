using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;

namespace RatesProvider.Handler.Infrastructure;

public class RetryPolicySettings : IRetryPolicySettings
{
    private readonly ILogger _logger;

    public RetryPolicySettings(ILogger<RetryPolicySettings> logger)
    {
        _logger = logger;
    }

    public ISyncPolicy BuildRetryPolicy() => Policy.Handle<Exception>()
            .WaitAndRetry(
            retryCount: Constant.CountRetry,
            sleepDurationProvider: (attemptCount) => TimeSpan.FromSeconds(Math.Pow(attemptCount, Constant.DelayMultiplier)),
            onRetry: (exception, sleepDuration, attemptNumber, context) =>
            _logger.LogInformation("{0}: retry with delay {1}, try {2}/{3}", exception, sleepDuration, attemptNumber, Constant.CountRetry));
}
