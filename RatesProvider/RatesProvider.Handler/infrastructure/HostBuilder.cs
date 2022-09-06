using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Interfaces;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace RatesProvider.Handler;

public class HostBuilder
{
    public static IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(Constant.SettingsDirectory);
        })
        .ConfigureServices((context, services) =>
        {

            services.AddScoped<IImplementation, Implementation>();
            services.AddScoped<ICurrencyHandler, CurrencyHandler>();
            services.AddScoped<IRatesBuilder, RatesBuilder>();
            services.AddScoped<ISettingsProvider, SettingsProvider>();
            services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
            services.AddScoped<IRetryPolicySettings, RetryPolicySettings>();

            services.AddLogging(loggingBuilder =>
             {
                 loggingBuilder.ClearProviders();
                 loggingBuilder.SetMinimumLevel(LogLevel.Information);
                 loggingBuilder.AddNLog();
             });
        });

        return hostBuilder;
    }
}
