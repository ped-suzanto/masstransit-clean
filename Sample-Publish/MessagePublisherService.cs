using Application;
using MessageContracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.ServiceBus
{
    public class MessagePublisherService : IHostedService
    {
        private readonly IServiceBus _serviceBus;

        public MessagePublisherService(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _serviceBus.StartAsync();

            int i = 0;
            do
            {
                ++i;
                var @event = new YourMessageEvent { CorrelationId = new Guid(), Database = $"balibible Event {i}", Table = "ads_type", Type = "insert", Ts = DateTime.Now, Xid = 162, Commit = true, Data = new Ads { Id = 11, Name = "test", Status = "active" } };

                await _serviceBus.Publish<UpdateIndex>(@event);

                var x = Console.ReadKey();

                if (x.KeyChar == 'q')
                {
                    break;
                }
            } while (true);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            Console.WriteLine("Bye");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _serviceBus.StopAsync();
        }
    }
}
