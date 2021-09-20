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
			var sessionIds = args.Length > 3 ? args[3] : "";

			//QueueReceiver receiver1 = new QueueReceiver(connectionString, queueName, id);
			SessionQueueReceiver receiver1 = new SessionQueueReceiver (connectionString, queueName, id, sessionIds);
			
			await receiver1.Go();
						
			Console.ReadLine();
		}
	}
}
