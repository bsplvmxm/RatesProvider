using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RatesProvider.Handler.Extensions;

namespace RatesProvider.Handler;

public class HostBuilder
{
    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());
        })
        .ConfigureServices((context, services) =>
        {
            services.ConfigureDependencies();
            services.ConfigureMessaging();
            services.ConfigureLogging();
        });
}
