using Application;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Hub
{
    public class RabbitMqHub : IHub, IDisposable
    {
        private readonly IBusControl _bus;

        public RabbitMqHub()
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://vulture.rmq.cloudamqp.com/itskeoul"), h =>
                {
                    h.Username("itskeoul");
                    h.Password("MgnokgkE3inCySH4EaAOpcjwyoik0LrH");
                });

            });

            _bus.Start();
        }

        public void Dispose()
        {
            _bus.Stop();
        }

        public async Task Publish<TMessage>(TMessage message) where TMessage : class
        {
            await _bus.Publish(message);
        }

        public async Task Send<TCommand>(object command) where TCommand : class
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://vulture.rmq.cloudamqp.com/itskeoul/commands"));
            await sendEndpoint.Send<TCommand>(command);
        }

        public async Task<TReturn> SendRequest<TCommand, TReturn>(TCommand command) where TCommand : class where TReturn : class
        {
            var client = _bus.CreateRequestClient<TCommand>(new Uri("rabbitmq://vulture.rmq.cloudamqp.com/itskeoul/command-requests"));
            var response = await client.GetResponse<TReturn>(command);

            return response.Message;
        }
    }
}
