using Microsoft.Extensions.DependencyInjection;
using RatesProvider.Handler;
using RatesProvider.Handler.Interfaces;

var host = HostBuilder.CreateHostBuilder().Build();

host.Services.GetService<IImplementation>()!.Run();