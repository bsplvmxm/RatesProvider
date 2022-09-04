using Polly.Retry;

namespace RatesProvider.Handler.Interfaces;

public interface IRetryPolicySettings
{
    RetryPolicy BuildRetryPolicy();
}
