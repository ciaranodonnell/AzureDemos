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

			var connectionString = "Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9JpjeQRtuHvQTAM0GRqA8UUnF+iiRr5rIpHFskL3RNE=";

			var queueName = "demoqueue";

			ServiceBusClient client = new ServiceBusClient(connectionString);


			var processor = client.CreateProcessor(queueName);
			processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
			processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
			await processor.StartProcessingAsync();


			var sender = client.CreateSender(queueName);

			var sendResult = await sender.ScheduleMessageAsync(new ServiceBusMessage("Hello to the future"), DateTimeOffset.Now.AddSeconds(5));

			Console.WriteLine($"{DateTime.Now.TimeOfDay} - Sent the message");


			Console.ReadLine();

			await sender.CancelScheduledMessageAsync(sendResult);

			Console.ReadLine();

		}

		private static Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
		{
			Console.WriteLine(arg.ToString());
			return Task.CompletedTask;
		}

		private async static Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
		{
			var message = arg.Message;
			Console.WriteLine($"{DateTime.Now.TimeOfDay} Received Processor Message: '{message.Body}'. Enqueued Time: {message.EnqueuedTime}");
			await arg.CompleteMessageAsync(message);
		}
	}
}
