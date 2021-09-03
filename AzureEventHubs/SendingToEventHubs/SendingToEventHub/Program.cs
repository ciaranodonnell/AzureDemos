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

			string namespaceConnectionString = "Endpoint=sb://youtubeeventhubdemo.servicebus.windows.net/;SharedAccessKeyName=sendandreceive;SharedAccessKey=Lz65+NZ5N0zwZRkvih+8kXfuD0tYWV7Sk1nNdY+xA+0=;EntityPath=demoeventhub";
			string eventHubName = "demoeventhub";

			await SendanEnumerableOfEvents(namespaceConnectionString, eventHubName);

			await SendaBatchOfEvents(namespaceConnectionString, eventHubName);


			Console.WriteLine("Sent the events");

			Console.ReadLine();

		}

		private static async Task SendaBatchOfEvents(string namespaceConnectionString, string eventHubName)
		{
			EventHubProducerClient producer = new EventHubProducerClient(namespaceConnectionString, eventHubName);

			var batch = await producer.CreateBatchAsync(new CreateBatchOptions { PartitionKey = "this is another string" });

			for (int i = 0; i < 10; i++)
			{
				batch.TryAdd(new EventData($"This is event {i}"));
			}

			await producer.SendAsync(batch);
		}



		private static async Task SendanEnumerableOfEvents(string namespaceConnectionString, string eventHubName)
		{
			EventHubProducerClient producer = new EventHubProducerClient(namespaceConnectionString, eventHubName);

			List<EventData> events = new List<EventData>();

			for (int i = 0; i < 10; i++)
			{
				events.Add(new EventData($"This is event {i}"));
			}

			await producer.SendAsync(events, new SendEventOptions { PartitionKey = "this is a string" });
		}


	}

}
