using Application;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Threading.Tasks;

namespace Infrastructure.ServiceBus
{
    public class RabbitMqBus : IServiceBus
    {
        protected readonly IBusControl _bus;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly RabbitMqConfiguration _configuration;

        public RabbitMqBus(IServiceProvider serviceProvider, RabbitMqConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(_configuration.Host), h =>
                {
                    h.Username(_configuration.Username);
                    h.Password(_configuration.Password);
                });

                Configure(cfg, host);
            });
        }

        protected virtual void Configure(IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host)
        {
        }

        public Task StartAsync()
        {
            return _bus.StartAsync();
        }

        public Task StopAsync()
        {
            return _bus.StopAsync();
        }

        public async Task Publish<TMessage>(TMessage message) where TMessage : class
        {
            await _bus.Publish(message);
        }

        public async Task Send<TCommand>(object command) where TCommand : class
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri($"{_configuration.Host}/${_configuration.CommandQueue}"));
            await sendEndpoint.Send<TCommand>(command);
        }

        public async Task<TReturn> SendRequest<TCommand, TReturn>(TCommand command) where TCommand : class where TReturn : class
        {
            var client = _bus.CreateRequestClient<TCommand>(new Uri($"{_configuration.Host}/${_configuration.CommandRequestQueue}"));
            var response = await client.GetResponse<TReturn>(command);

            return response.Message;
        }
    }
}
