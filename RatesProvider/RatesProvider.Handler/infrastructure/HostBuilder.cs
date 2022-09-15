using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RatesProvider.Handler.Extensions;
using RatesProvider.Recipient.Infrastructure;

namespace RatesProvider.Handler;

public class HostBuilder
{
    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(Environment.GetEnvironmentVariable(EnvironmentVirable.BaseDirectory, EnvironmentVariableTarget.Machine)!);
        })
        .ConfigureServices((context, services) =>
        {
            services.ConfigureDependicies();
            services.ConfigureMessaging();
            services.ConfigureLogging();
        });
}
