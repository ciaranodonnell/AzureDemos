using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ConsoleApp1
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString =
				"<connection string>";

			var topicName = "demotopic";

			TopicClient topicClient = new TopicClient(connectionString, topicName, RetryPolicy.Default);

			SubscriptionClient subscriptionClient = new SubscriptionClient(connectionString, topicName,
				"demosubscription1", ReceiveMode.PeekLock, RetryPolicy.Default);
			
			subscriptionClient.RegisterMessageHandler((message, cancelToken) =>
				{
					var bodyBytes = message.Body;

					var ourMessage = System.Text.Encoding.UTF8.GetString(bodyBytes);

					Console.WriteLine("Message Received:" + ourMessage);

					return Task.CompletedTask;
				},
				(exceptionArg) =>
				{

					Console.WriteLine("Exception Occurred! : " + exceptionArg.Exception.ToString());

					return Task.CompletedTask;
				});


			while (true)
			{
				var messageText = Console.ReadLine();

				if (messageText == "quit") break;
				
				var ourMessage = System.Text.Encoding.UTF8.GetBytes(messageText);

				await topicClient.SendAsync(new Message(ourMessage));

			}
		}
	}
}