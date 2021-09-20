using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private List<string> sessionIds;

		public SessionQueueReceiver(string connectionString, string queueName, string id, string sessionIds)
		{
			this.connectionString = connectionString;
			this.queueName = queueName;
			this.id = id;
			if (!string.IsNullOrWhiteSpace(sessionIds))
			{
				this.sessionIds = sessionIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
			}
		}



		public async Task Go()
		{
			//Debugger.Launch();
			Console.WriteLine($"Starting Processor '{id}'");

			this.client = new ServiceBusClient(connectionString);

			var options = new ServiceBusSessionProcessorOptions
			{
				ReceiveMode = ServiceBusReceiveMode.PeekLock,
				// Processing can be optionally limited to a subset of session Ids.
				MaxConcurrentSessions = 2
				 
			};

			if (sessionIds?.Count > 0)
			{
				sessionIds.ForEach(sessionId => options.SessionIds.Add(sessionId));
				Console.WriteLine("Starting with Session Ids: " + string.Join(',', sessionIds));
			}

			// create a session processor that we can use to process the messages
			this.processor = client.CreateSessionProcessor(queueName, options);

			// configure the message and error handler to use
			processor.SessionInitializingAsync += Processor_SessionInitializingAsync;
			processor.ProcessMessageAsync += MessageHandler;
			processor.ProcessErrorAsync += ErrorHandler;
			processor.SessionClosingAsync += Processor_SessionClosingAsync;

			// start processing
			await processor.StartProcessingAsync();


			Console.WriteLine($"Processor '{id}' started");
		}

		private Task Processor_SessionClosingAsync(ProcessSessionEventArgs arg)
		{
			Console.WriteLine($"Processor_SessionClosingAsync called with Session Id: {arg.SessionId}");
			return Task.CompletedTask;
		}

		private Task Processor_SessionInitializingAsync(ProcessSessionEventArgs arg)
		{
			Console.WriteLine($"Processor_SessionInitializingAsync called with Session Id: {arg.SessionId}");
			return Task.CompletedTask;
		}

		async Task MessageHandler(ProcessSessionMessageEventArgs args)
		{
			var body = args.Message.Body.ToString();

			var sessionStateData = (await args.GetSessionStateAsync());
			CustomSessionStateObject sessionState;

			if (sessionStateData == null)
			{
				sessionState = new CustomSessionStateObject();
			}
			else
			{
				sessionState = sessionStateData.ToObjectFromJson<CustomSessionStateObject>();
			}

			sessionState.MessagesOnSession++;

			Console.WriteLine($"Processor {id}, Session {args.SessionId}, Session Msg# {sessionState.MessagesOnSession}, Body: {body}");

			await args.SetSessionStateAsync(BinaryData.FromObjectAsJson(sessionState));


			// we can evaluate application logic and use that to determine how to settle the message.
			await args.CompleteMessageAsync(args.Message);


		}

		public class CustomSessionStateObject
		{
			public int MessagesOnSession { get; set; }
		}

		Task ErrorHandler(ProcessErrorEventArgs args)
		{
			Console.WriteLine("Error procesing Message:");
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
