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



            string namespaceConnectionString = "Endpoint=sb://eventhubyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=demoeventhubreader;SharedAccessKey=3oNnV3Rfq7YOXpUqgA10qI8RgsqUnt+6E+JXFHVtD7E=;EntityPath=demoeventhub";
            string eventHubName = "demoeventhub";


            string blobEndpoint = "DefaultEndpointsProtocol=https;AccountName=youtubedemostorage;AccountKey=2Tv1I3o4nRhSulZINRMr9G62S8GFup61BPS1SjJ+Q8aLWIqtH1rB/hqbBkSuQzL4wDWeizts2rslim6SgnMH/Q==;EndpointSuffix=core.windows.net";


            blobStorage = new BlobContainerClient(blobEndpoint,"eventhubcontainer");

            await StartSingleReceiver(namespaceConnectionString, eventHubName);

            Console.WriteLine("Processor Started");

            Console.ReadLine();

            client.StopProcessing();
        }


        public static async Task StartSingleReceiver(string namespaceConnectionString, string eventHubName)
        {
            
           client = new(blobStorage,"$Default", namespaceConnectionString);

            client.ProcessEventAsync += ConsumeEvent;
            client.ProcessErrorAsync += Client_ProcessErrorAsync;
            await client.StartProcessingAsync();
        }

        private static Task Client_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            Console.WriteLine($"Event Error Occurred. {JsonConvert.SerializeObject(arg)} ");

            return Task.CompletedTask;
        }

        public static Task ConsumeEvent(ProcessEventArgs e)
        {
            Console.WriteLine($"Event Received. P={e.Partition.PartitionId}, B={e.Data.EventBody}");

            //return e.UpdateCheckpointAsync();
            return Task.CompletedTask;
        }


    }
}
