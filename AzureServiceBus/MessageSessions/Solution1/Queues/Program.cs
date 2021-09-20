using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace CompetingConsumersQueues
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString = "";

			var queueName = "sessionedqueue";

			ServiceBusClient client = new ServiceBusClient(connectionString);
			var sender = client.CreateSender(queueName);


			var consumerId = 1;

			List<IDisposable> d = new List<IDisposable>();
			//d.Add(ConsumerAppProxy.StartConsumerApp(connectionString, queueName, consumerId++));

			const int numberOfCustomers = 10;
			Random customerNumberPicker = new Random();


			ThreadPool.QueueUserWorkItem(async (state) =>
			{
				int messsageNumber = 0;


				while (messsageNumber < 1000)
				{
					try
					{
						var customerId = customerNumberPicker.Next(numberOfCustomers);
						var messageText = $"Message number {messsageNumber} is about Customer {customerId}";

						var ourMessage = System.Text.Encoding.UTF8.GetBytes(messageText);
						var msg = new ServiceBusMessage(ourMessage);

						//Assign the message to a session for a customer
						msg.SessionId = customerId.ToString();

						await sender.SendMessageAsync(msg);
						await Task.Delay(250);
						messsageNumber++;
					}
					catch (Exception ex) { Console.WriteLine(ex.ToString()); }
				}
			});

			while (true)
			{
				try
				{
					var messageText = Console.ReadLine();

					if (messageText == "quit") break;

					if (messageText.StartsWith("up"))
					{
						string sessionIds = "";
						if (messageText.StartsWith("up "))
						{
							sessionIds = messageText.Substring(3);
						}
						d.Add(ConsumerAppProxy.StartConsumerApp(connectionString, queueName, (consumerId++).ToString(), sessionIds));
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
						var msg = new ServiceBusMessage(ourMessage);

						msg.SessionId = "typed";

						await sender.SendMessageAsync(msg);
					}
				}
				catch (Exception ex) { Console.WriteLine(ex.ToString()); }
			}

			foreach (var oneToKill in d)
			{
				oneToKill.Dispose();
			}
		}
	}
}