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
	public class TcsBasedCommunicationService : ICommunicationService
	{
		readonly MessagingConfig config;
		TopicClient topicClient;
		SubscriptionClient subscriptionClient;

		public TcsBasedCommunicationService(MessagingConfig config)
		{
			this.config = config;

			this.topicClient = new TopicClient(config.ConnectionString, config.RequestTopic);

			this.subscriptionClient = new SubscriptionClient(config.ConnectionString, config.ReplyTopic, config.ReplySubscription);


			subscriptionClient.RegisterMessageHandler(HandleMessage, HandleError);


		}

		
		Task HandleError(ExceptionReceivedEventArgs args)
		{
			return Task.CompletedTask;
		}


		Dictionary<Guid, StateObj> states = new Dictionary<Guid, StateObj>();

		Task HandleMessage(Message message, CancellationToken cancelToken)
		{

			var response = JsonConvert.DeserializeObject<ResponseMessage>(System.Text.Encoding.UTF8.GetString(message.Body));
			if (states.TryGetValue(response.Id, out var state))
			{
				state.taskSource.SetResult(new GetNumberResult { Id = response.Id, Number = response.Number });
				states.Remove(response.Id);
			}
			return Task.CompletedTask;
		}


		public async Task<GetNumberResult> DoAsyncWork(Guid Id)
		{

			StateObj state = new StateObj { Id = Id, taskSource = new TaskCompletionSource<GetNumberResult>() };

			states.Add(Id, state);

			var send = topicClient.SendAsync(new Message(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RequestMessage { Id = Id }))));

			await Task.WhenAll(send, state.taskSource.Task);

			return state.taskSource.Task.Result;

		}

		class StateObj
		{
			public Guid Id;
			public TaskCompletionSource<GetNumberResult> taskSource;
			public int Number;
			public int LoopCount;
			public bool IsCompleted;
		}
	}
}