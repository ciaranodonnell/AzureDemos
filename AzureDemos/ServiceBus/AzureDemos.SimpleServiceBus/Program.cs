using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
			var namespaceConnectionString =
				"Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=plVg4/vEV4mlBWn1i5WU1FatmtxeHvbAOLqRIIHU8c0=";


			await BasicReceiveMessage(namespaceConnectionString, queuePath);


			while (true)
			{
				await BasicSendMessage(namespaceConnectionString, queuePath);
				var entry = Console.ReadLine();
				if (entry == "quit") break;
			}
		}

		private static Task BasicReceiveMessage(string namespaceConnectionString, string queuePath)
		{
			QueueClient ourQueue =
				new QueueClient(namespaceConnectionString, queuePath, ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);

			ourQueue.RegisterMessageHandler((message, cancelToken) =>
			{
				Console.WriteLine(Encoding.UTF8.GetString(message.Body));
				return Task.CompletedTask;
			}, (exceptionEventArgs) =>
			{
				return Console.Error.WriteLineAsync(exceptionEventArgs.Exception.ToString());
			});
			return Task.CompletedTask;
		}


		private static int messagenumber = 0;
		private static async Task BasicSendMessage(string namespaceConnectionString, string queuePath)
		{
			
			ManagementClient mgtClient = new ManagementClient(namespaceConnectionString);

			if ((await mgtClient.QueueExistsAsync(queuePath)) == false)
			{
				var queueDescription = new QueueDescription(queuePath);
				queueDescription.MaxDeliveryCount = 10;
				queueDescription.DefaultMessageTimeToLive = TimeSpan.MaxValue;
				queueDescription.EnableDeadLetteringOnMessageExpiration = false;

				await mgtClient.CreateQueueAsync(queueDescription);
			}

			QueueClient ourQueue =
				new QueueClient(namespaceConnectionString, queuePath, ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);

			var messageBytes = Encoding.UTF8.GetBytes($"This is message {messagenumber}");
			messagenumber++;
			await ourQueue.SendAsync(new Message(messageBytes));
		}
	}
}