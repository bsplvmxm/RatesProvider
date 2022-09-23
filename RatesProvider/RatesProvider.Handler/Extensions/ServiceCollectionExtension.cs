using IncredibleBackend.Messaging;
using IncredibleBackend.Messaging.Extentions;
using IncredibleBackend.Messaging.Interfaces;
using IncredibleBackendContracts.Constants;
using IncredibleBackendContracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;

namespace RatesProvider.Handler.Extensions;

public static class ServiceCollectionExtension
{
    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IImplementation, Implementation>();
        services.AddScoped<ICurrencyHandler, CurrencyHandler>();
        services.AddScoped<IRatesBuilder, RatesBuilder>();
        services.AddScoped<ISettingsProvider, SettingsProvider>();
        services.AddScoped<IRetryPolicySettings, RetryPolicySettings>();
        services.AddScoped<IMessageProducer, MessageProducer>();
    }

    public static void ConfigureMessaging(this IServiceCollection services)
    {
        services.RegisterConsumersAndProducers(
                null,
                null,
                (cfg) => { cfg.RegisterProducer<NewRatesEvent>(RabbitEndpoint.CurrencyRates); },
                Environment.GetEnvironmentVariable("RABBIT_LOGIN", EnvironmentVariableTarget.Machine)!,
                Environment.GetEnvironmentVariable("RABBIT_PASSWORD", EnvironmentVariableTarget.Machine)!,
                Environment.GetEnvironmentVariable("RABBIT_SERVER", EnvironmentVariableTarget.Machine)!);
    }

    public static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
            loggingBuilder.AddNLog();
        });
    }
}
