using Infrastructure.Hub;
using MessageContracts;
using System;
using System.Threading.Tasks;

namespace Sample_Publish
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using(var bus = new RabbitMqHub())
            {
                var command = new YourMessageCommand { CorrelationId = new Guid(), Database = "balibible", Table = "ads_type", Type = "insert", Ts = DateTime.Now, Xid = 162, Commit = true, Data = new Ads { Id = 11, Name = "test", Status = "active" } };
                await bus.Send<UpdateIndex>(new { CorrelationId = new Guid(), Database = "balibible" });
                var result = await bus.SendRequest<YourMessageCommand, YourMessageCommand>(command);

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Console.WriteLine("Bye");
            }
        }
    }
}