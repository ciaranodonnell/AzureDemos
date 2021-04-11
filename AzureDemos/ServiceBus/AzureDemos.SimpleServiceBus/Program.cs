using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace AzureDemos.SimpleServiceBus
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting our Service Bus Demo!");

			
			var queuePath = "ourqueue";
			string namespaceConnectionString = string.Empty;
			
			ServiceBusConnection connection = new ServiceBusConnection(namespaceConnectionString);
			
			ManagementClient mgtClient = new ManagementClient(namespaceConnectionString);
			
			if ((await mgtClient.QueueExistsAsync(queuePath)) == false)
			{
				var queueDescription = new QueueDescription(queuePath);
				queueDescription.MaxDeliveryCount = 10;
				queueDescription.DefaultMessageTimeToLive = TimeSpan.MaxValue;
				queueDescription.EnableDeadLetteringOnMessageExpiration = false;
				
				await  mgtClient.CreateQueueAsync(queueDescription);
			}

			QueueClient ourQueue =
				new QueueClient(connection, queuePath, ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);

			var messageBytes = Encoding.UTF8.GetBytes("This is our first message");
			await ourQueue.SendAsync(new Message(messageBytes));
			
		}
	}
}