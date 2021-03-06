﻿using System;
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
		private static MessageReceiver dlqMessageReceiver;


		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString =
	"";
			var topicName = "demotopic";
			var subscriptionName = "demosubscription1";

			var queueName = "demoqueue";



			//I have added the below line to give us a clear subscription each time. Dont worry about how it works for now
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
				AutoComplete = false,
			};

			subscriptionClient.RegisterMessageHandler(MessageHandler, options);


			var subscriptionPath = EntityNameHelper.FormatSubscriptionPath(topicName, subscriptionName);

			var dlqEntityPath = EntityNameHelper.FormatDeadLetterPath(subscriptionPath);

			dlqMessageReceiver = new MessageReceiver(connectionString, dlqEntityPath);

			MessageHandlerOptions dlqOption = new MessageHandlerOptions(ExceptionHandler)
			{
				AutoComplete = false
			};

			dlqMessageReceiver.RegisterMessageHandler(DLQMessageHandler, dlqOption);


			Console.ReadLine();

		}

		static async Task DLQMessageHandler(Message message, CancellationToken token)
		{
			var bodyBytes = message.Body;

			var ourMessage = System.Text.Encoding.UTF8.GetString(bodyBytes);

			Console.WriteLine($"DLQ Message Received: '{ourMessage}'. Id = {message.MessageId}. Delivery Count = {message.SystemProperties.DeliveryCount}");

			await dlqMessageReceiver.CompleteAsync(message.SystemProperties.LockToken);
		}



		static async Task MessageHandler(Message message, CancellationToken token)
		{
			var bodyBytes = message.Body;

			var ourMessage = System.Text.Encoding.UTF8.GetString(bodyBytes);

			Console.WriteLine($"Message Received and DeadLettered: '{ourMessage}'. Id = {message.MessageId}");

			if (message.SystemProperties.DeliveryCount < 5)
			{
				await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
			}
			else
			{
				await subscriptionClient.DeadLetterAsync(message.SystemProperties.LockToken);
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