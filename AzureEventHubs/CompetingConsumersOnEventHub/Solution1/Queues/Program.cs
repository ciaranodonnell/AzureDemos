using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;


namespace CompetingConsumersEventHub
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Event Hubs Demo");

			var connectionString =
				"Endpoint=sb://youtubeeventhubdemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=";

			var eventHubName = "demoeventhubwith4partitions";
			//var eventHubName = "demoeventhub";
			var consumerGroup = "$Default";


			EventHubProducerClient client = new EventHubProducerClient(connectionString, eventHubName);

			var consumerId = 1;

			List<IDisposable> d = new List<IDisposable>();
			d.Add(ConsumerAppProxy.StartConsumerApp(connectionString, eventHubName, consumerGroup, consumerId++));


			ThreadPool.QueueUserWorkItem(async (state) =>
			{
				int messsageNumber = 0;

				Random r = new Random();

				while (messsageNumber < 1000)
				{

					var messageText = $"Message number {messsageNumber} at {DateTime.UtcNow.TimeOfDay}";

					var ourMessage = System.Text.Encoding.UTF8.GetBytes(messageText);
					var msg = new EventData(ourMessage);
					await client.SendAsync(new EventData[] { msg }, new SendEventOptions { PartitionKey = messsageNumber.ToString() });
					await Task.Delay(r.Next(500));
					messsageNumber++;

				}
			});

			while (true)
			{
				var messageText = Console.ReadLine();

				if (messageText == "quit") break;

				if (messageText == "up")
				{
					d.Add(ConsumerAppProxy.StartConsumerApp(connectionString, eventHubName, consumerGroup, consumerId++));
					continue;
				}

				if (messageText == "down")
				{
					var oneToKill = d[d.Count - 1];
					d.RemoveAt(d.Count - 1);
					oneToKill.Dispose();
					continue;
				}

				foreach (var bit in messageText.Split(' '))
				{
					var ourMessage = System.Text.Encoding.UTF8.GetBytes(bit);
					var msg = new EventData(ourMessage);
					await client.SendAsync(new EventData[] { msg }, new SendEventOptions { PartitionKey = messageText });
				}
			}



		}
	}
}