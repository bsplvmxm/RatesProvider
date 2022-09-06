using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;

namespace RatesProvider.Handler.Infrastructure;

public class RetryPolicySettings : IRetryPolicySettings
{
    private readonly ILogger<RetryPolicySettings> _logger;

    public RetryPolicySettings(ILogger<RetryPolicySettings> logger)
    {
        _logger = logger;
    }

    public RetryPolicy BuildRetryPolicy() => Policy.Handle<Exception>()
            .WaitAndRetry(
            retryCount: Constant.CountRetry,
            sleepDurationProvider: (attemptCount) => TimeSpan.FromSeconds(attemptCount * Constant.DelayMultiplier),//В СТЕПЕНЬ 
            onRetry: (exception, sleepDuration, attemptNumber, context) =>
            _logger.LogInformation("{0}: retry with delay {1}, try {2}/{3}", exception, sleepDuration, attemptNumber, Constant.CountRetry));
}
