using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace SendingToEventHub
{
    class Program
    {
        static int messageNumber = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting our Event Hub Producer");

            string namespaceConnectionString = "Endpoint=sb://eventhubyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=sendpolicy;SharedAccessKey=0GeKILsXQNlvXmxA0uZ1j5kkRAdMAsqPEcD6qB0asZ8=";
            string eventHubName = "demoeventhub";

            
            await SendAFewMessages(namespaceConnectionString, eventHubName);
            Console.WriteLine("Messages Sent");
            Console.ReadLine();
        }



        public static async Task SendAFewMessages(string namespaceConnectionString, string eventHubName)
        {

            EventHubProducerClient client = new EventHubProducerClient(namespaceConnectionString, eventHubName);

            //var batch = await client.CreateBatchAsync(new CreateBatchOptions { PartitionKey = "hello" });
            List<EventData> events = new();
            EventData[] messages = new EventData[1];
            for (int x = 0; x < 10; x++)
            {
                var greetingPart = (x % 2 == 0 ? "Hello " : "World! ");
                var msgText = $"{ messageNumber++} - {greetingPart}";
                var msg = new Azure.Messaging.EventHubs.EventData(msgText);
                messages[0] = msg;
                await client.SendAsync(messages, new SendEventOptions { PartitionId = (x%2).ToString()});

            }


        }
    }
}
