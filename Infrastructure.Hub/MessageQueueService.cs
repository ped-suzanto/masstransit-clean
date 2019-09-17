using Application;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Hub
{
    public class MessageQueueService : BackgroundService, IHub
    {
        private readonly IBusControl _bus;
        private readonly IServiceProvider _serviceProvider;

        public MessageQueueService(IServiceProvider serviceProvider)
        {
            var contextName = AppDomain.CurrentDomain.FriendlyName;
            Type eventHandlerType = typeof(IEventHandler<>);
            Type commandHandlerType = typeof(ICommandHandler<>);
            Type requestHandlerType = typeof(IRequestHandler<,>);

            _serviceProvider = serviceProvider;
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://vulture.rmq.cloudamqp.com/itskeoul"), h =>
                {
                    h.Username("itskeoul");
                    h.Password("MgnokgkE3inCySH4EaAOpcjwyoik0LrH");
                });

                var handlerTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => 
                    !x.IsAbstract && 
                    !x.IsInterface && 
                    x.GetInterfaces().Any(y => 
                        y.IsGenericType && 
                        (y.GetGenericTypeDefinition() == eventHandlerType ||
                        y.GetGenericTypeDefinition() == commandHandlerType ||
                        y.GetGenericTypeDefinition() == requestHandlerType
                        )))
                .GroupBy(x => x.GetInterfaces()[0].GetGenericTypeDefinition())
                .ToDictionary(x => x.Key, x=> x.ToList());

                if (handlerTypes.ContainsKey(eventHandlerType))
                {
                    ConfigureHandlers(cfg, host, AppDomain.CurrentDomain.FriendlyName, eventHandlerType, handlerTypes[eventHandlerType]);
                }

                if (handlerTypes.ContainsKey(commandHandlerType))
                {
                    ConfigureHandlers(cfg, host, "commands", commandHandlerType, handlerTypes[commandHandlerType]);
                }

                if (handlerTypes.ContainsKey(requestHandlerType))
                {
                    ConfigureHandlers(cfg, host, "command-requests", requestHandlerType, handlerTypes[requestHandlerType]);
                }
            });
        }

        private void ConfigureHandlers(IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host, string channel, Type genericHandlerType, IEnumerable<Type> handlerTypes)
        {
            cfg.ReceiveEndpoint(host, channel, ep =>
            {
                var endPointType = typeof(HandlerExtensions);

                foreach (var handlerType in handlerTypes)
                {
                    var handlerGenericArguments = handlerType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == genericHandlerType).GetGenericArguments();
                    var messageType = handlerGenericArguments[0];
                    var handlerObject = _serviceProvider.GetService(handlerType);
                    var messageHandlerType = handlerGenericArguments.Length > 1 ? typeof(MessageHandlerFactory<,>) : typeof(MessageHandlerFactory<>);
                    var messageHandlerFactory = Activator.CreateInstance(messageHandlerType.MakeGenericType(handlerGenericArguments), handlerObject);
                    var handleDelegateMethod = messageHandlerFactory.GetType().GetMethod("HandleDelegate");
                    var handlerMethod = endPointType.GetMethod("Handler");
                    var genericHandlerMethod = handlerMethod.MakeGenericMethod(messageType);

                    genericHandlerMethod.Invoke(ep, new object[] { ep, handleDelegateMethod.Invoke(messageHandlerFactory, null), null });
                }
            });
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _bus.StartAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(base.StopAsync(cancellationToken), _bus.StopAsync());
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
