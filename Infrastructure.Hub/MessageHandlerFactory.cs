using Application;
using MassTransit;

namespace Infrastructure.ServiceBus
{
    public class MessageHandlerFactory<TMessage> where TMessage : class
    {
        private IMessageHandler<TMessage> _handler;

        public MessageHandlerFactory(IMessageHandler<TMessage> handler)
        {
            _handler = handler;
        }

        public MessageHandler<TMessage> HandleDelegate()
        {
            return async delegate (ConsumeContext<TMessage> context)
            {
                await _handler.Handle(context.Message);
            };
        }
    }

    public class MessageHandlerFactory<TMessage, TReturn> where TMessage : class where TReturn : class
    {
        private IRequestHandler<TMessage, TReturn> _handler;

        public MessageHandlerFactory(IRequestHandler<TMessage, TReturn> handler)
        {
            _handler = handler;
        }

        public MessageHandler<TMessage> HandleDelegate()
        {
            return async delegate (ConsumeContext<TMessage> context)
            {
                await context.RespondAsync(await _handler.Handle(context.Message));
            };
        }
    }
}
