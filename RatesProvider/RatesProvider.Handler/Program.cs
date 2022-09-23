using Microsoft.Extensions.DependencyInjection;
using RatesProvider.Handler;
using RatesProvider.Handler.Interfaces;


var host = HostBuilder.CreateHostBuilder().Build();

host.Services.CreateScope();

await host.Services.GetService<IImplementation>()!.Run();

while (true)
{
    Thread.Sleep(10000);
}