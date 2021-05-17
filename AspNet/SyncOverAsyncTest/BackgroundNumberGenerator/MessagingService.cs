using Common;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundNumberGenerator
{
	public class MessagingService
	{
		readonly MessagingConfig config;
		TopicClient topicClient;
		SubscriptionClient subscriptionClient;

		public MessagingService(MessagingConfig config)
		{
			this.config = config;
			
			this.topicClient = new TopicClient(config.ConnectionString, config.ReplyTopic);

			this.subscriptionClient = new SubscriptionClient(config.ConnectionString, config.RequestTopic, config.RequestSubscription);

			MessageHandlerOptions options = new MessageHandlerOptions(HandleError);
			options.AutoComplete = true;
			options.MaxAutoRenewDuration = TimeSpan.FromSeconds(600);

			subscriptionClient.RegisterMessageHandler(HandleMessage, options);


		}

		async Task HandleMessage(Message message, CancellationToken cancelToken)
		{

			var request = JsonConvert.DeserializeObject<RequestMessage>(System.Text.Encoding.UTF8.GetString(message.Body));

			Console.WriteLine($"Enter an integer for Request {request.Id}");
			var i = int.Parse(Console.ReadLine());

			await topicClient.SendAsync(new Message(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ResponseMessage { Id = request.Id, Number=i }))));
			
		}


		Task HandleError(ExceptionReceivedEventArgs args)
		{
			return Task.CompletedTask;
		}



	}
}
