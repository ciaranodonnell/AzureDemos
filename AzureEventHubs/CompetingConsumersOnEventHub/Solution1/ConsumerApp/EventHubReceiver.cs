using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompetingConsumersEventHub
{
	class EventHubReceiver
	{
		readonly string eventHubName;
		private readonly string consumerGroup;
		readonly string connectionString;
		string id;

		//use this to simulate random errors
		Random random;


		public EventHubReceiver(string connectionString, string eventHubName, string consumerGroup, string id)
		{
			this.connectionString = connectionString;
			this.id = id;
			this.eventHubName = eventHubName;
			this.consumerGroup = consumerGroup;

			this.random = new Random();

		}




		public async Task Go()
		{
			Console.WriteLine($"Starting our Event Hub Receiver {id}");

			string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=eventhubstorageaccount3;AccountKey=;EndpointSuffix=core.windows.net";
			string containerName = "offsetcontainer";


			BlobContainerClient blobContainerClient = new BlobContainerClient(blobConnectionString, containerName);

			EventProcessorClient processor = new EventProcessorClient(blobContainerClient, "$Default", connectionString, eventHubName);

			processor.ProcessEventAsync += Processor_ProcessEventAsync;
			processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

			await processor.StartProcessingAsync();
			Console.WriteLine("Started the processor");


			Console.ReadLine();
			await processor.StopProcessingAsync();
			Console.WriteLine("Started the processor");

		}


		private static Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
		{
			Console.WriteLine("Error Received: " + arg.Exception.ToString());
			return Task.CompletedTask;
		}

		private static async Task Processor_ProcessEventAsync(ProcessEventArgs arg)
		{
			Console.WriteLine($"Event Received from Partition {arg.Partition.PartitionId}: {arg.Data.EventBody.ToString()}");

			await arg.UpdateCheckpointAsync();

		}

	}
}
