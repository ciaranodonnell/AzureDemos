using Common;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SyncOverAsyncTest
{
	public class CommunicationService
	{
		readonly MessagingConfig config;
		TopicClient topicClient;
		SubscriptionClient subscriptionClient;

		public CommunicationService(MessagingConfig config)
		{
			this.config = config;

			this.topicClient = new TopicClient(config.ConnectionString, config.RequestTopic);

			this.subscriptionClient = new SubscriptionClient(config.ConnectionString, config.ReplyTopic, config.ReplySubscription);


			subscriptionClient.RegisterMessageHandler(HandleMessage, HandleError);


		}

		Task HandleMessage(Message message, CancellationToken cancelToken)
		{

			var response = JsonConvert.DeserializeObject<ResponseMessage>(System.Text.Encoding.UTF8.GetString(message.Body));
			if (states.TryGetValue(response.Id, out var state))
			{
				state.Number = response.Number;
				state.token.Cancel();
			}
			return Task.CompletedTask;
		}


		Task HandleError(ExceptionReceivedEventArgs args)
		{
			return Task.CompletedTask;
		}


		Dictionary<Guid, StateObj> states = new Dictionary<Guid, StateObj>();


		public async Task<GetNumberResult> DoAsyncWork(Guid Id, Func<Task> keepAlive)
		{

			StateObj state = new StateObj { Id = Id, token = new CancellationTokenSource() };

			states.Add(Id, state);

			await topicClient.SendAsync(new Message(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RequestMessage { Id = Id }))));


			while (state.LoopCount < 10 && !state.IsCompleted)
			{
				try
				{
					await Task.Delay(1000, state.token.Token);
					if (keepAlive is not null)
						await keepAlive();
				}
				catch (TaskCanceledException) { state.IsCompleted = true; }
				state.LoopCount++;
			}

			states.Remove(Id);

			return new GetNumberResult { Id = Id, Number = state.Number };

		}






		class StateObj
		{
			public Guid Id;
			public CancellationTokenSource token;
			public int Number;
			public int LoopCount;
			public bool IsCompleted;
		}


	}
}
