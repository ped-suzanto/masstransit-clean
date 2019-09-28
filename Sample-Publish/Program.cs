using Infrastructure.ServiceBus;
using MessageContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Sample_Publish
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddServiceBus();
                  services.AddHostedService<MessagePublisherService>();
              });

            await builder
              .RunConsoleAsync();
        }
    }
}