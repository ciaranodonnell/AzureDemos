using AzureDemos.ServiceBus.DemoApp.BusTools;
using AzureDemos.ServiceBus.DemoApp.Verbs;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureDemos.ServiceBus.DemoApp
{
    public class CompetingConsumerExample
    {
        private CompetingConsumerTestVerb config;
        private ServiceBusManager mgt;

        public CompetingConsumerExample(CompetingConsumerTestVerb config)
        {

            this.config = config;

            mgt = new ServiceBusManager(config.ConnectionString);
        }


        public async Task RunTest()
        {


            var topic = "competing" + (new Random().Next()).ToString();
            var subscriptionName = "subscriptionTo" + topic;

            await mgt.CreateTopic(topic);


            await mgt.CreateSubscription(topic, subscriptionName);


            var sender = new TopicClient(config.ConnectionString, topic);

            Microsoft.Azure.ServiceBus.SubscriptionClient receiver1 = new SubscriptionClient(config.ConnectionString, topic, subscriptionName, ReceiveMode.ReceiveAndDelete);
            receiver1.RegisterMessageHandler((message, ct) =>
            {
                Console.WriteLine("Receved On Receiver 1");
                System.Threading.Thread.Sleep(500);
                return Task.CompletedTask;
            }, new MessageHandlerOptions((e) => Task.CompletedTask));


            Microsoft.Azure.ServiceBus.SubscriptionClient receiver2 = new SubscriptionClient(config.ConnectionString, topic, subscriptionName, ReceiveMode.ReceiveAndDelete);
            receiver2.RegisterMessageHandler((message, ct) =>
            {
                Console.WriteLine("Receved On Receiver 2");
                System.Threading.Thread.Sleep(500);
                return Task.CompletedTask;
            }, new MessageHandlerOptions((e) => Task.CompletedTask));


            for (int x = 0; x < 10; x++)
               await sender.SendAsync(new Message { Body = new byte[0], CorrelationId = Guid.NewGuid().ToString() });

        }


    }
}
