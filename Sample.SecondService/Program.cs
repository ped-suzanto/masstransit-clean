using Infrastructure.Hub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Sample.SecondService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
             .ConfigureServices((hostContext, services) =>
             {
                 services.AddTransient<YourMessageCommandHandler>();
                 services.AddTransient<YourMessageSecondHandler>();
                 services.AddBusConsumerServices();
             });

            await builder
              .RunConsoleAsync();
        }
    }
}
