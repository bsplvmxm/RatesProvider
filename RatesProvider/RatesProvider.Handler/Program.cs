using Microsoft.Extensions.DependencyInjection;
using RatesProvider.Handler;
using RatesProvider.Handler.Interfaces;

var host = HostBuilder.CreateHostBuilder().Build();

await host.Services.GetService<IImplementation>()!.Run();

while (true)
{
    Thread.Sleep(10000);
}