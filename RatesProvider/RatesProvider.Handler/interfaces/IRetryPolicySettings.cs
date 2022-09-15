using Polly;

namespace RatesProvider.Handler.Interfaces;

public interface IRetryPolicySettings
{
    ISyncPolicy BuildRetryPolicy();
}
