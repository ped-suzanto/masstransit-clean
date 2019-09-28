using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ServiceBus
{
    public class RabbitMqConfiguration
    {
        public RabbitMqConfiguration(IConfigurationRoot configuration)
        {
            if (configuration != null)
            {
                Host = "";
                Username = "";
                Password = "";
                CommandQueue = "";
                CommandRequestQueue = "";
            }
            else
            {
                Host = "rabbitmq://vulture.rmq.cloudamqp.com/itskeoul";
                Username = "itskeoul";
                Password = "MgnokgkE3inCySH4EaAOpcjwyoik0LrH";
                CommandQueue = "command";
                CommandRequestQueue = "command-request";
            }
        }

        public RabbitMqConfiguration()
            : this(null)
        {
        }

        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CommandQueue { get; set; }
        public string CommandRequestQueue { get; set; }
    }
}
