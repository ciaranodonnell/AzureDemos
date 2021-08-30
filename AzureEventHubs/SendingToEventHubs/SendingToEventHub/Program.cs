using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace SendingToEventHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting our Event Hub Producer");

            string namespaceConnectionString = "Endpoint=sb://eventhubyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=sendpolicy;SharedAccessKey=0GeKILsXQNlvXmxA0uZ1j5kkRAdMAsqPEcD6qB0asZ8=";
            string eventHubName = "demoeventhub";

            await SendAFewMessages(namespaceConnectionString, eventHubName);

            await SendAFewMessages(namespaceConnectionString, eventHubName);


        }


        public static async Task SendABatchOfMessages(string namespaceConnectionString, string eventHubName)
        {

            EventHubProducerClient client = new EventHubProducerClient(namespaceConnectionString, eventHubName);

            var batch = await client.CreateBatchAsync(new CreateBatchOptions { PartitionKey = "world" });
            
            for (int x = 0; x < 10; x++)
            {
                var msg = new Azure.Messaging.EventHubs.EventData("This is an event message");
                batch.TryAdd(msg);
            }

            await client.SendAsync(batch);

        }

        public static async Task SendAFewMessages(string namespaceConnectionString, string eventHubName)
        {

            EventHubProducerClient client = new EventHubProducerClient(namespaceConnectionString, eventHubName);

            //var batch = await client.CreateBatchAsync(new CreateBatchOptions { PartitionKey = "hello" });
            List<EventData> events = new();

            for (int x = 0; x < 10; x++)
            {
                var msg = new Azure.Messaging.EventHubs.EventData("This is an event message");
                events.Add(msg);
            }

            //batch.TryAdd(message);

            await client.SendAsync(events, new SendEventOptions { PartitionKey = "hello" });

        }
    }
}
