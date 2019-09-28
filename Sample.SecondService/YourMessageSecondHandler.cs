using Application;
using MessageContracts;
using System;
using System.Threading.Tasks;

namespace Sample.SecondService
{
    public class YourMessageSecondHandler : IRequestHandler<YourMessageEvent, YourMessageEvent>
    {
        public async Task<YourMessageEvent> Handle(YourMessageEvent message)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;
            await Console.Out.WriteLineAsync($"Received 2nd - {appName} : {message.Database}");

            return message;
        }
    }
}
