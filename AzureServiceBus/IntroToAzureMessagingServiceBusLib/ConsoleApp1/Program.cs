using System;
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

			var topicName = "demotopic";
			var subscriptionName = "demosubscription";


			ServiceBusClient client = new ServiceBusClient(connectionString);


			var sender = client.CreateSender(queueName);

			await sender.SendMessageAsync(new ServiceBusMessage("This is a single message that we sent"));

			var batch = await sender.CreateMessageBatchAsync();
			for (int x = 0; x < 1000; x++)
			{
				batch.TryAddMessage(new ServiceBusMessage($"This is message {x} that we sent"));
			}

			await sender.SendMessagesAsync(batch);


			var receiver = client.CreateReceiver(queueName);

			var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
			if (message != null)
			{
				Console.WriteLine("Received Single Message: " + message.Body);
				await receiver.CompleteMessageAsync(message);
			}
			else
			{
				Console.WriteLine("Didnt receive a message");
			}

			//var messageEnum = receiver.ReceiveMessagesAsync();

			//await foreach(var msg in messageEnum)
			//{

			//	Console.WriteLine("Received Multiple Message: " + msg.Body);
			//	await receiver.CompleteMessageAsync(msg);
			//}
			try
			{

				var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions { PrefetchCount = 50, MaxConcurrentCalls = 10 });
				processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
				processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
				await processor.StartProcessingAsync();

			}
			catch (Exception ex) { Console.WriteLine(ex.ToString()); }


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
			Console.WriteLine("Received Processor Message: " + message.Body);
			await arg.CompleteMessageAsync(message);
		}
	}
}
