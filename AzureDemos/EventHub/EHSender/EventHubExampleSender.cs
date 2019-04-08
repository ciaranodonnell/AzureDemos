using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureDemos.EventHub.EHSender
{
    class EventHubExampleSender
    {

        public EventHubExampleSender(string EventHubConnectionString)
        {
            this.connectionString = EventHubConnectionString;

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
            

        }

        private static EventHubClient eventHubClient;

        private string connectionString;
        //private const string EventHubName = "{Event Hub path/name}";

        public async Task SendMessages(int numMessagesToSend)
        {
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    var message = $"Message {i}";
                    Console.WriteLine($"Sending message: {message}");
                    var bytesToSend = Encoding.UTF8.GetBytes(message);
                    var eventData = new EventData(bytesToSend);

                    await eventHubClient.SendAsync(eventData, "partition1");
                    
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }
        }

    }
}
