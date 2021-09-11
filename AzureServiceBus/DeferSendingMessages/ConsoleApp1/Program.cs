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

			string connectionString = "Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=SendDemo;SharedAccessKey=";
			string queueName = "demoqueue";
			string topicName = "demotopic";
			string subscriptionName = "demotopicsub1";


			ServiceBusClient client = new ServiceBusClient(connectionString);



			var processor = client.CreateProcessor(topicName, subscriptionName);
			processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
			processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
			await processor.StartProcessingAsync();


			var sender = client.CreateSender(topicName);

			ServiceBusMessage message = new ServiceBusMessage($"New Message Created at {DateTime.Now.TimeOfDay}");

			long sequenceNumber = await sender.ScheduleMessageAsync(message, DateTimeOffset.Now.AddSeconds(75));

			Console.WriteLine($"Scheduled a message with sequence number {sequenceNumber}");

			Console.ReadLine();

			await sender.CancelScheduledMessageAsync(sequenceNumber);

			Console.WriteLine("Cancellation Requested");


			var receiver = client.CreateReceiver(topicName, subscriptionName);
			var peekedMessage = await receiver.PeekMessageAsync();

			if (peekedMessage != null)
			{
				Console.WriteLine($"{DateTime.Now.TimeOfDay} Peeked Message: '{message.Body}'. Enqueued Time: {peekedMessage.EnqueuedTime}");
			}
			else
			{
				Console.WriteLine("Peeked for a message but nothing was there");
			}


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
			Console.WriteLine(arg.Exception.ToString());
			return Task.CompletedTask;
		}

	}
}
