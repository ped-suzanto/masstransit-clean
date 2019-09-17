using System.Threading.Tasks;

namespace Application
{
    public interface IRequestHandler<TMessage, TResponse> where TMessage : class where TResponse : class
    {
        Task<TResponse> Handle(TMessage message);
    }
}
