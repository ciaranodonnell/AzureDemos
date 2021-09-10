using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace ConsoleApp1
{
	static class Program
	{

		static async Task Main(string[] args)
		{

			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString = "Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=demoqueuepolicy;SharedAccessKey=M4Nm0T/Br/hHAP6OvrpFHJAbtpwy+VrTyEgm7deahAA=;EntityPath=demoqueue";

			var queueName = "demoqueue";

			ServiceBusClient client = new ServiceBusClient(connectionString);


			var processor = client.CreateProcessor(queueName);
			processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
			processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
			await processor.StartProcessingAsync();


			var sender = client.CreateSender(queueName);

			var sequenceNumber = await sender.ScheduleMessageAsync(new ServiceBusMessage($"New Message from {DateTime.UtcNow.TimeOfDay}")
				, DateTimeOffset.UtcNow.AddSeconds(30));

			Console.WriteLine("Message Scheduled - " + sequenceNumber.ToString());


			Console.ReadLine();


			var receiver = client.CreateReceiver(queueName);

			var peekedMessage = await receiver.PeekMessageAsync();

			Console.WriteLine($"{DateTime.Now.TimeOfDay} Peeked Message: '{peekedMessage.Body}'. Enqueued Time: {peekedMessage.EnqueuedTime}");

			
			await sender.CancelScheduledMessageAsync(sequenceNumber);
			Console.WriteLine("Scheduled Message Cancellation Requested - " + sequenceNumber.ToString());


			Console.ReadLine();


		}


		private async static Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
		{
			var message = arg.Message;
			Console.WriteLine($"{DateTime.Now.TimeOfDay} Received Processor Message: '{message.Body}'. Enqueued Time: {message.EnqueuedTime}");
			await arg.CompleteMessageAsync(message);
		}
		private static Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
		{
			Console.WriteLine(arg.ToString());
			return Task.CompletedTask;
		}

	}
}
