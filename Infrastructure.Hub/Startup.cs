using Application;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Hub
{
    public static class Startup
    {
        public static void AddBusServices(this IServiceCollection services)
        {
            services.AddSingleton<IHub>();
        }

        public static void AddBusConsumerServices(this IServiceCollection services)
        {
            services.AddHostedService<MessageQueueService>();
        }
    }
}
