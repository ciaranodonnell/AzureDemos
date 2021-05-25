using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace CompetingConsumersQueues
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString =
				"Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=SendDemo;SharedAccessKey=qQpXvuApzeF2n6tN3ZOoK5kIOgbCCn8N+PH2XVZ4Lwc=";

			var queueName = "demoQueue";


			QueueClient sendingClient = new QueueClient(connectionString, queueName, ReceiveMode.PeekLock, RetryPolicy.Default);

			var consumerId = 1;

			List<IDisposable> d = new List<IDisposable>();
			d.Add(ConsumerAppProxy.StartConsumerApp(connectionString, queueName, consumerId++));


			ThreadPool.QueueUserWorkItem(async (state) =>
			{
				int messsageNumber = 0;

				Random r = new Random();

				while (messsageNumber < 1000)
				{

					var messageText = $"Message number {messsageNumber}";

					var ourMessage = System.Text.Encoding.UTF8.GetBytes(messageText);
					var msg = new Message(ourMessage);
					await sendingClient.SendAsync(msg);
					await Task.Delay(r.Next(250));
					messsageNumber++;

				}
			});
						
			while (true)
			{
				var messageText = Console.ReadLine();

				if (messageText == "quit") break;

				if (messageText == "up")
				{
					d.Add(ConsumerAppProxy.StartConsumerApp(connectionString, queueName, consumerId++));
				}
				if (messageText == "down")
				{
					var oneToKill = d[d.Count - 1];
					d.RemoveAt(d.Count - 1);
					oneToKill.Dispose();
				}

				foreach (var bit in messageText.Split(' '))
				{
					var ourMessage = System.Text.Encoding.UTF8.GetBytes(bit);
					var msg = new Message(ourMessage);
					await sendingClient.SendAsync(msg);
				}
			}

			

		}
	}
}