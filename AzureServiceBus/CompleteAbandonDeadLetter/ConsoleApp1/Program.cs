using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;

namespace ConsoleApp1
{
	class Program
	{
		private static SubscriptionClient subscriptionClient;

		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString =
				"";
			var topicName = "demotopic";
			var subscriptionName = "demosubscription1";

			await ClearSubscription(connectionString, topicName, subscriptionName);
			
			var topicClient = new TopicClient(connectionString, topicName);

			for (int x = 1; x <= 10; x++)
			{
				var messageText = $"Message {x}";

				var ourMessage = System.Text.Encoding.UTF8.GetBytes(messageText);

				await topicClient.SendAsync(new Message(ourMessage));

			}

			Console.WriteLine("Sent 10 messages");


			subscriptionClient = new SubscriptionClient(connectionString, topicName, subscriptionName, ReceiveMode.PeekLock);

			MessageHandlerOptions options = new MessageHandlerOptions(ExceptionHandler)
			{
				MaxAutoRenewDuration = TimeSpan.Zero,
				AutoComplete =  false,
			};

			subscriptionClient.RegisterMessageHandler(MessageHandler, options);
			
			Console.ReadLine();

		}

		private static readonly HashSet<string> previousMessageIds = new HashSet<string>();
		private static int receivedMessageNumber = 0;
		static async Task MessageHandler(Message message, CancellationToken token)
		{
			var bodyBytes = message.Body;

			var ourMessage = System.Text.Encoding.UTF8.GetString(bodyBytes);
			
			var seenBefore = previousMessageIds.Contains(message.MessageId);

			if (!seenBefore) previousMessageIds.Add(message.MessageId);

			receivedMessageNumber++;
			
			if (receivedMessageNumber  % 2 == 0)
			{
				Console.WriteLine($"Message Received: '{ourMessage}'. Number {receivedMessageNumber}. Rejected the message");
				await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
			}
			else
			{
				Console.WriteLine($"Message Received: '{ourMessage}'. Number {receivedMessageNumber}. Completing the message");
				await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
			}
		}


		static Task ExceptionHandler(ExceptionReceivedEventArgs exceptionArgs)
		{

			Console.WriteLine("Exception Occurred! : " + exceptionArgs.Exception.ToString());

			return Task.CompletedTask;
		}

		static async Task ClearSubscription(string connectionString, string topic, string subscription)
		{

			ManagementClient mc = new ManagementClient(connectionString);
			var sub = await mc.GetSubscriptionAsync(topic, subscription);

			await mc.DeleteSubscriptionAsync(topic, subscription);

			await mc.CreateSubscriptionAsync(sub);
			
		}
	}
}