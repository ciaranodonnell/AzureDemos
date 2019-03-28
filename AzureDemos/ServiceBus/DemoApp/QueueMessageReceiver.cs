using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;

namespace AzureDemos.ServiceBus.DemoApp.BusTools
{
	class QueueMessageReceiver
	{
		private string connectionString;
		private ManagementClient mgt;
		private string queuePath;
		private QueueClient client;

		public QueueMessageReceiver(string serviceBusConnectionString, string queueName, bool peekAndLock)
		{
			this.connectionString = serviceBusConnectionString;
			mgt = new ManagementClient(serviceBusConnectionString);
			queuePath = queueName;
			client = new QueueClient(serviceBusConnectionString, queueName, peekAndLock ? ReceiveMode.PeekLock : ReceiveMode.ReceiveAndDelete);



		}

		public async Task<int> GetActiveQueueLength()
		{
			var rtInfo = await mgt.GetQueueRuntimeInfoAsync(queuePath);
			return (int)rtInfo.MessageCountDetails.ActiveMessageCount;

		}

		public async Task<int> GetDeadLetterCount()
		{
			var rtInfo = await mgt.GetQueueRuntimeInfoAsync(queuePath);
			return (int)rtInfo.MessageCountDetails.DeadLetterMessageCount;

		}


		public void StartReceivingMessages()
		{
			client.RegisterMessageHandler(new Func<Message, System.Threading.CancellationToken, Task>(HandleMessage), new MessageHandlerOptions(new Func<ExceptionReceivedEventArgs, Task>(ErrorHandler)) { AutoComplete = false });

		}

		internal async Task<string> GetDeadLetterMessage()
		{
			var dlqName = EntityNameHelper.FormatDeadLetterPath(queuePath);
			var message = await (new MessageReceiver(connectionString, dlqName, receiveMode: ReceiveMode.ReceiveAndDelete).ReceiveAsync(1, TimeSpan.FromSeconds(1)));
			var msg = message?.FirstOrDefault();
					   
			return msg == null ? null : UTF8Encoding.UTF8.GetString(msg.Body);
		}

		public async Task HandleMessage(Message msg, CancellationToken token)
		{

			var body = UTF8Encoding.UTF8.GetString(msg.Body);

			var handler = HandleMessageAndReturnIfSuccess;
			bool shouldComplete = false;
			if (handler != null)
			{
				shouldComplete = handler(body);
			}
			if (shouldComplete)
			{
				Console.WriteLine("Completing Message");
				await client.CompleteAsync(msg.SystemProperties.LockToken);
			}
			else
			{
				Console.WriteLine("Abandoning Message");
				await client.AbandonAsync(msg.SystemProperties.LockToken);
			}

		}
		public Task ErrorHandler(ExceptionReceivedEventArgs e)
		{
			Console.WriteLine($"Exception Received: {e.Exception.ToString()}");
			return Task.CompletedTask;
		}


		public event Func<string, bool> HandleMessageAndReturnIfSuccess;


	}
}
