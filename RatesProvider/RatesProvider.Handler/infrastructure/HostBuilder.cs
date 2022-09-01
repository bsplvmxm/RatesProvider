using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Interfaces;
using NLog.Web;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

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
            services.AddScoped<IRatesBuilder, RatesBuilder>();
            services.AddScoped<IRatesGetter, Recipient.RatesGetter>();
            services.AddScoped<ICurrencyHandler, CurrencyHandler>();
            services.AddScoped<IImplementation, Implementation>();
            services.AddScoped<ISettingsProvider, SettingsProvider>();
            services.AddLogging();

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
