using CompetingConsumersEventHub;

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
			var consumerGroup = args[2];
			var id = args.Length > 3 ? args[3] : "";

			EventHubReceiver receiver1 = new EventHubReceiver(connectionString, queueName, id, consumerGroup);
			await receiver1.Go();

			Console.ReadLine();
		}
	}
}
