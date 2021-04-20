using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace ConsoleApp1
{
	class Program
	{
		private static SubscriptionClient subscriptionClient;

		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString =
				"Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=SendDemo;SharedAccessKey=p6RX8MAehjbA6SOw56ZcvxB1al7lQDudWYbSUcDsdQg=";

			var topicName = "demotopic";

			var subscriptionName = "demosubscription1";



			var topicClient = new TopicClient(connectionString, topicName);

			for (int x = 0; x < 10; x++)
			{
				var messageText = $"Message {x}";

				var ourMessage = System.Text.Encoding.UTF8.GetBytes(messageText);

				await topicClient.SendAsync(new Message(ourMessage));

			}

			Console.WriteLine("Sent 10 messages");


			subscriptionClient = new SubscriptionClient(connectionString, topicName, subscriptionName, ReceiveMode.PeekLock);

			MessageHandlerOptions options = new MessageHandlerOptions(ExceptionHandler)
			{
				MaxAutoRenewDuration = TimeSpan.Zero
			};


			subscriptionClient.RegisterMessageHandler(MessageHandler, options);


			Console.ReadLine();

		}

		static async Task MessageHandler(Message message, CancellationToken token)
		{
			var bodyBytes = message.Body;

			var ourMessage = System.Text.Encoding.UTF8.GetString(bodyBytes);


			var lockedUntil = message.SystemProperties.LockedUntilUtc;

			var lockToken = message.SystemProperties.LockToken;

			Console.WriteLine($"1- Message Received: '{ourMessage}'. Locked for {(lockedUntil - DateTime.UtcNow).TotalSeconds}");


			await Task.Delay(6000);


			Console.WriteLine($"1- Message Finished: '{ourMessage}'. Locked for {(message.SystemProperties.LockedUntilUtc - DateTime.UtcNow).TotalSeconds}");


		}


		static Task ExceptionHandler(ExceptionReceivedEventArgs exceptionArgs)
		{

			Console.WriteLine("Exception Occurred! : " + exceptionArgs.Exception.ToString());

			return Task.CompletedTask;
		}
	}
}