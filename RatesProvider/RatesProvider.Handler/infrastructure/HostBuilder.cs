using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Recipient;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler;

public class HostBuilder
{
    public static IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());
        })
        .ConfigureServices((context, services) =>
        {
            services.AddScoped<IRatesBuilder, RatesBuilder>();
            services.AddScoped<IRatesGetter, RatesGetter>();
            services.AddScoped<ICurrencyHandler, CurrencyHandler>();
            services.AddScoped<IImplementation, Implementation>();
        });

        return hostBuilder;
    }
}
