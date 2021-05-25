using Microsoft.Azure.ServiceBus;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompetingConsumersQueues
{
	class QueueReceiver
	{
		readonly string queueName;
		readonly string connectionString;
		string id;

		//use this to simulate random errors
		Random random;


		public QueueReceiver(string connectionString, string queueName, string id)
		{
			this.connectionString = connectionString;
			this.id = id;
			this.queueName = queueName;
			this.random = new Random();

		}




		public void Go()
		{
			QueueClient receivingClient = new QueueClient(connectionString, queueName, ReceiveMode.PeekLock, RetryPolicy.Default);
			
			MessageHandlerOptions options = new MessageHandlerOptions(HandleError)
			{
				MaxConcurrentCalls = 1
			};

			receivingClient.RegisterMessageHandler(HandleMessage, options);


		}

		public Task HandleMessage(Message message, CancellationToken cancelToken)
		{
			lock (Console.Out)
			{
				var bodyBytes = message.Body;

				var ourMessage = System.Text.Encoding.UTF8.GetString(bodyBytes);
				Console.WriteLine($"Message Received on {id}: {ourMessage}");


				//if (random.Next(5) == 0) throw new Exception("oops");

			}
			return Task.CompletedTask;

		}

		public Task HandleError(ExceptionReceivedEventArgs args)
		{
			Console.WriteLine($"Exception Occurred on {id}! : {args.Exception}");

			return Task.CompletedTask;

		}


	}
}
