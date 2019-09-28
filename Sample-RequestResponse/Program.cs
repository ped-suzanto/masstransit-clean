using Infrastructure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleRequestResponse;
using System.Threading.Tasks;

namespace Sample_RequestResponse
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddTransient<YourMessageCommandHandler>();
                  services.AddServiceBusConsumers();
              });

            await builder
              .RunConsoleAsync();
        }
    }
}