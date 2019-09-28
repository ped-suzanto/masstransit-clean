using Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.ServiceBus
{
    public static class Startup
    {
        public static void AddServiceBus(this IServiceCollection services)
        {
            services.AddSingleton<IServiceBus, RabbitMqBus>();
        }

        public static void AddServiceBusConsumers(this IServiceCollection services)
        {
            services.AddHostedService<MessageQueueService>();
        }

        public static void UseRabbitMqBus(IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            var serviceBus = app.ApplicationServices.GetService<IServiceBus>();

            serviceBus.StartAsync();

            applicationLifetime.ApplicationStopping.Register(() => OnShutdown(serviceBus));
        }

        private static void OnShutdown(IServiceBus serviceBus)
        {
            serviceBus.StopAsync();
        }
    }
}
