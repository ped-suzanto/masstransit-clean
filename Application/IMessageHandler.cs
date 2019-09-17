using System.Threading.Tasks;

namespace Application
{
    public interface IMessageHandler<TMessage>
    {
        Task Handle(TMessage message);
    }
}
