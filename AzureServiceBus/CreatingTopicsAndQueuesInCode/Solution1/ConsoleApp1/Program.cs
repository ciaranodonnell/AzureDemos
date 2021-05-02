using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace ConsoleApp1
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Starting Azure Service Bus Demo");

			var connectionString =
				"";


			var queueName = "myNewQueue";

			var topicName = "myNewTopic";

			var subscriptionName = "myNewSubscription";


			ManagementClient managementClient = new ManagementClient(connectionString);


			if (await managementClient.QueueExistsAsync(queueName))
				await managementClient.DeleteQueueAsync(queueName);



			var queueDescription = new QueueDescription(queueName);

			queueDescription.LockDuration = TimeSpan.FromSeconds(5);
			queueDescription.MaxDeliveryCount = 5;

			queueDescription = await managementClient.CreateQueueAsync(queueDescription);


			var topicDescription = new TopicDescription(topicName);

			await managementClient.CreateTopicAsync(topicDescription);

			var subscriptionDescription = new SubscriptionDescription(topicName, subscriptionName);

			subscriptionDescription.MaxDeliveryCount = 6;
			subscriptionDescription.LockDuration = TimeSpan.FromSeconds(6);


			await managementClient.CreateSubscriptionAsync(subscriptionDescription);


			var topics = await managementClient.GetTopicsAsync();
			
			foreach(var topic in topics)
			{
				Console.WriteLine(topic.Path);
			}


			var subscriptionDesc = await managementClient.GetSubscriptionAsync(topicName, subscriptionName);
			await managementClient.DeleteSubscriptionAsync(topicName, subscriptionName);
			await managementClient.CreateSubscriptionAsync(subscriptionDesc);


			var nameSpaceInfo = await managementClient.GetNamespaceInfoAsync();

			


		}

		

	}
}