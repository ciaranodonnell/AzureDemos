﻿using CompetingConsumersQueues;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace ConsumerApp
{
	public class Program
	{
		static async Task Main(string[] args)
		{

			var connectionString = args[0];
				
			var queueName = args[1];

			var id = args.Length > 2 ? args[2] : "";

			//QueueReceiver receiver1 = new QueueReceiver(connectionString, queueName, args[2]);
			//receiver1.Go();

			SessionQueueReceiver sessionReceiver = new SessionQueueReceiver(connectionString, queueName, id);
			await sessionReceiver.Start();

			Console.ReadLine();
		}
	}
}
