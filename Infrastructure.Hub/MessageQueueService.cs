using Application;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.ServiceBus
{
    public class MessageQueueService : RabbitMqBus, IHostedService
    {
        public MessageQueueService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override void Configure(IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host)
        {
            var contextName = AppDomain.CurrentDomain.FriendlyName;
            Type eventHandlerType = typeof(IEventHandler<>);
            Type commandHandlerType = typeof(ICommandHandler<>);
            Type requestHandlerType = typeof(IRequestHandler<,>);

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
                            .ToDictionary(x => x.Key, x => x.ToList());

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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus.StopAsync();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _bus.StartAsync();
        }
    }
}
