using Application;
using MessageContracts;
using System;
using System.Threading.Tasks;

namespace SampleRequestResponse
{
    public class YourMessageCommandHandler : ICommandHandler<UpdateIndex>
    {
        public async Task Handle(UpdateIndex message)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;
            await Console.Out.WriteLineAsync($"Received - {appName} : {message.Database}");
        }
    }
}
