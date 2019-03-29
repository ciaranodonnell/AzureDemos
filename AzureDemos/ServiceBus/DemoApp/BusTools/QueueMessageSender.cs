using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace AzureDemos.ServiceBus.DemoApp.BusTools
{
    class QueueMessageSender
    {
        private readonly string connectionString;
        private QueueClient client;
        private readonly string queueName;

        public QueueMessageSender(string serviceBusConnectionString, string queueName)
        {
            this.connectionString = serviceBusConnectionString;

            this.client = new QueueClient(serviceBusConnectionString, queueName);
                        
            this.queueName = queueName;
        }

        public async Task SendMessage(string messageContents)
        {
            await client.SendAsync(new Message { Body = UTF8Encoding.UTF8.GetBytes(messageContents), TimeToLive = TimeSpan.FromSeconds(10) });
        }





    }
}
