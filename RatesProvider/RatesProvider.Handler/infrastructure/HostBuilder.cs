using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.Extensions.Logging;
using RatesProvider.Recipient.Infrastructure;
using MassTransit;
using IncredibleBackendContracts.Constants;
using IncredibleBackendContracts.Events;

namespace RatesProvider.Handler;

public class HostBuilder
{
    public static IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(Environment.GetEnvironmentVariable(EnvironmentVirable.BaseDirectory, EnvironmentVariableTarget.Machine)!);
        })
        .ConfigureServices((context, services) =>
        {

            services.AddScoped<IImplementation, Implementation>();
            services.AddScoped<ICurrencyHandler, CurrencyHandler>();
            services.AddScoped<IRatesBuilder, RatesBuilder>();
            services.AddScoped<ISettingsProvider, SettingsProvider>();
            services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
            services.AddScoped<IRetryPolicySettings, RetryPolicySettings>();

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseNewtonsoftJsonSerializer();

                    cfg.Host(Environment.GetEnvironmentVariable(EnvironmentVirable.RabbitServer, EnvironmentVariableTarget.Machine), h =>
                    {
                        h.Username(Environment.GetEnvironmentVariable(EnvironmentVirable.RabbitLogin));
                        h.Password(Environment.GetEnvironmentVariable(EnvironmentVirable.RabbitPassword));
                    });

                    cfg.ReceiveEndpoint(RabbitEndpoint.CurrencyRates, e => e.Bind<NewRatesEvent>());
                });
            });

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
