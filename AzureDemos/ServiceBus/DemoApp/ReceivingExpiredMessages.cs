using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureDemos.ServiceBus.DemoApp.BusTools;
using AzureDemos.ServiceBus.DemoApp.Verbs;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace AzureDemos.ServiceBus.DemoApp
{
    class ReceivingExpiredMessages
    {
        private ExpiredMessagesTestVerb config;
        private ServiceBusManager mgt;

        public ReceivingExpiredMessages(ExpiredMessagesTestVerb config)
        {
            this.config = config;

            mgt = new ServiceBusManager(config.ConnectionString);

        }


        public async Task RunTest()
        {
            Console.WriteLine("About to Create Queue named " + config.QueuePath);
            await mgt.CreateQueueAsync(new QueueDescription(config.QueuePath) { DefaultMessageTimeToLive = TimeSpan.FromSeconds(5), EnableDeadLetteringOnMessageExpiration = false }, true); ;

            Console.WriteLine("Queue Created. Sending a message");
            QueueMessageSender sender = new QueueMessageSender(config.ConnectionString, config.QueuePath);
            QueueMessageReceiver receiver = new QueueMessageReceiver(config.ConnectionString, config.QueuePath, false);

            await sender.SendMessage("first message");

            Console.WriteLine("Message Sent, Going to receive it now");
            string message = await receiver.GetSingleMessage();

            Console.WriteLine("Message Received: " +( message ?? "<NULL>"));


            Console.WriteLine("Sending second message");
            await sender.SendMessage("second message");

            Console.WriteLine("Message Sent, Going to sleep for 6 seconds");
            Thread.Sleep(6000);

            Console.WriteLine("I'm Back, receiving message");
             message = await receiver.GetSingleMessage();


            Console.WriteLine("Message Received: " + (message ?? "<NULL>"));

            Console.WriteLine("Hit Enter to Continue and Delete Queue");
            Console.ReadLine();

            await mgt.DeleteQueueAsync(config.QueuePath);


        }
    }
}
