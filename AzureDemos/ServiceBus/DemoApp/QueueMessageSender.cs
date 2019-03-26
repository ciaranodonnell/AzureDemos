using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace AzureDemos.ServiceBus.DemoApp
{
	class QueueMessageSender
	{
		private string connectionString;
		private QueueClient client;
		private ManagementClient mgt;
		private string queueName;

		public QueueMessageSender(string serviceBusConnectionString, string queueName)
		{
			this.connectionString = serviceBusConnectionString;

			this.client = new Microsoft.Azure.ServiceBus.QueueClient(serviceBusConnectionString, queueName);

			mgt = new ManagementClient(serviceBusConnectionString);

			this.queueName = queueName;
		}

		public async Task SendMessage(string messageContents)
		{
			if (!(await mgt.QueueExistsAsync(queueName)))
			{
				await mgt.CreateQueueAsync(new QueueDescription(queueName) { DefaultMessageTimeToLive = TimeSpan.FromSeconds(5), MaxDeliveryCount = 1, EnableDeadLetteringOnMessageExpiration=true });
			}
			await client.SendAsync(new Message { Body = UTF8Encoding.UTF8.GetBytes(messageContents), TimeToLive = TimeSpan.FromSeconds(10) });
		}





	}
}
