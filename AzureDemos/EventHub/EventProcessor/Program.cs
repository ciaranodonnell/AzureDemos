using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;

namespace AzureDemos.EventHub.EventProcessor
{
    class Program
    {
        private static EventProcessorHost host;

        static void Main(string[] args)
        {




        }
        public static void SetupEventProcessorHost(string connectionString, string eventHubPath, string consumerGroupName, string storageConnectionString, string leaseContainerName)
        {
            host = new EventProcessorHost(eventHubPath,
                consumerGroupName,
                connectionString,
                storageConnectionString,
                leaseContainerName
                );

            host.RegisterEventProcessorAsync<SimpleEventProcessor>(new EventProcessorOptions
            {
                InitialOffsetProvider = (partitionId) =>
//EventPosition.FromSequenceNumber(msgSqNm)
//EventPosition.FromEnd()
//EventPosition.FromEnqueuedTime(DateTime.UtcNow)
//EventPosition.FromOffset(offsetString, !skipThisOneAndGoToNext)
//EventPosition.FromSequenceNumber(msgSqNm)
EventPosition.FromStart()
            });
        }
    }
}
