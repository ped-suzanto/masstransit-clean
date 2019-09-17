namespace Application
{
    public interface ICommandHandler<TMessage> : IMessageHandler<TMessage>
    {
    }
}
