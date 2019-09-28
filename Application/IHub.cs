using System;
using System.Threading.Tasks;

namespace Application
{
    public interface IServiceBus
    {
        Task StartAsync();
        Task StopAsync();
        Task Publish<TMessage>(TMessage message) where TMessage : class;
        Task Send<TCommand>(object command) where TCommand : class;
        Task<TReturn> SendRequest<TCommand, TReturn>(TCommand command) where TCommand : class where TReturn : class;
    }
}
