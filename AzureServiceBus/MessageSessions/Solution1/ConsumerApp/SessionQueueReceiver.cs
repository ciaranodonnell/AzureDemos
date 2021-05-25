using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerApp
{
	class SessionQueueReceiver
	{
		private string connectionString;
		private string queueName;
		private string id;
		private ServiceBusSessionProcessor processor;
		private ServiceBusClient client;

		public SessionQueueReceiver(string connectionString, string queueName, string id)
		{
			this.connectionString = connectionString;
			this.queueName = queueName;
			this.id = id;
		}



		public async Task Start()
		{
			this.client = new ServiceBusClient(connectionString);

			var options = new ServiceBusSessionProcessorOptions
			{
				ReceiveMode = ServiceBusReceiveMode.PeekLock,
				// Processing can be optionally limited to a subset of session Ids.
	//			MaxConcurrentSessions=2
			};

			if (id.Length > 0) options.SessionIds.Add(id);

			// create a session processor that we can use to process the messages
			this.processor = client.CreateSessionProcessor(queueName, options);

			// configure the message and error handler to use
			processor.SessionInitializingAsync += Processor_SessionInitializingAsync;
			processor.ProcessMessageAsync += MessageHandler;
			processor.ProcessErrorAsync += ErrorHandler;
			processor.SessionClosingAsync += Processor_SessionClosingAsync;

			// start processing
			await processor.StartProcessingAsync();

			Console.WriteLine($"Processing Started on '{id}'");
		}

		private Task Processor_SessionClosingAsync(ProcessSessionEventArgs arg)
		{
			Console.WriteLine($"Processor_SessionClosingAsync called on {arg.SessionId}");
			return Task.CompletedTask;
		}

		private Task Processor_SessionInitializingAsync(ProcessSessionEventArgs arg)
		{
			Console.WriteLine($"Processor_SessionInitializingAsync called on {arg.SessionId}");
			return Task.CompletedTask;
		}

		async Task MessageHandler(ProcessSessionMessageEventArgs args)
		{
			var body = args.Message.Body.ToString();
			Console.WriteLine($"Message Received on {id}: {body}");


			// we can evaluate application logic and use that to determine how to settle the message.
			await args.CompleteMessageAsync(args.Message);


		}

		Task ErrorHandler(ProcessErrorEventArgs args)
		{
			// the error source tells me at what point in the processing an error occurred
			Console.WriteLine(args.ErrorSource);
			// the fully qualified namespace is available
			Console.WriteLine(args.FullyQualifiedNamespace);
			// as well as the entity path
			Console.WriteLine(args.EntityPath);
			Console.WriteLine(args.Exception.ToString());
			return Task.CompletedTask;
		}
	}
}
