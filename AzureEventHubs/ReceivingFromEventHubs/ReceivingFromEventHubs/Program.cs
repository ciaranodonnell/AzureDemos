using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace SendingToEventHub
{
	class Program
	{
		private static BlobContainerClient blobStorage;
		private static Azure.Messaging.EventHubs.EventProcessorClient client;

		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting our Event Hub Receiver");



			string namespaceConnectionString = "Endpoint=sb://eventhubyoutubedemos2.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KoaJlFSB5wTdAnwoHKrQA+fl2eoWHpjyBK9vkuo8aFw=";
			string eventHubName = "demoeventhub";


			string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=eventhubstorageaccount2;AccountKey=QybIK0ll3m4TGvb5mB7RbP3BfRSDGssvGrJ+hU1vNldSVahJ/ankfp55zGsNkMItu5cjZPpyaVfffrrPJ8p1vQ==;EndpointSuffix=core.windows.net";
			string containerName = "offsetcontainer";


			blobStorage = new BlobContainerClient(blobConnectionString, containerName);

			client = new EventProcessorClient(blobStorage, "$Default", namespaceConnectionString, eventHubName);

			client.ProcessEventAsync += Client_ProcessEventAsync;
			client.ProcessErrorAsync += Client_ProcessErrorAsync;

			await client.StartProcessingAsync();
			Console.WriteLine("Processor Started");


			Console.ReadLine();
			await client.StopProcessingAsync();
			Console.WriteLine("Processor Stopped");
		}

		static int messagesProcessed = 0;
		private static async Task Client_ProcessEventAsync(ProcessEventArgs arg)
		{
			Console.WriteLine($"Event Received from {arg.Partition.PartitionId}: {arg.Data.EventBody.ToString()}");

			messagesProcessed++;
			if (messagesProcessed % 10 == 0)
			{
				await arg.UpdateCheckpointAsync();
			}

		}


		private static Task Client_ProcessErrorAsync(ProcessErrorEventArgs arg)
		{
			Console.WriteLine("An Error Occurred. " + arg.Exception.ToString());
			return Task.CompletedTask;
		}

	}
}
