using AzureDemos.ServiceBus.DemoApp.BusTools;
using AzureDemos.ServiceBus.DemoApp.Verbs;
using CommandLine;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace AzureDemos.ServiceBus.DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<ExpiredMessagesTest, BasicSendReceiveTestVerb>(args)
                .MapResult(
                  (BasicSendReceiveTestVerb opts) => DoBasicTest(opts),
                  (ExpiredMessagesTest opts) => DoExpiredMessagesTest(opts),
                  errs => 1);

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static int DoExpiredMessagesTest(ExpiredMessagesTest opts)
        {

            return 0;
        }

        static int DoBasicTest(BasicSendReceiveTestVerb opts)
        {
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["asbcs"];
            string queueName = System.Configuration.ConfigurationManager.AppSettings["demoqueue"];

            DoSendAndReceive(connectionString, queueName).GetAwaiter().GetResult();

            return 0;
        }






        public static async Task DoSendAndReceive(string connectionString, string queueName)
        {
            var sender = new QueueMessageSender(connectionString, queueName);
            var receiver = new QueueMessageReceiver(connectionString, queueName, true);


            Console.WriteLine($"Queue Length = {receiver.GetActiveQueueLength()}");

            await sender.SendMessage("message 1");
            await sender.SendMessage("message 2");
            await sender.SendMessage("message 3");
            await sender.SendMessage("message 4");
            await sender.SendMessage("message 5");
            await sender.SendMessage("message 6");

            Console.WriteLine($"Queue Length = {await receiver.GetActiveQueueLength()}");



            int numToFail = 3;
            int numFailed = 0;
            receiver.HandleMessageAndReturnIfSuccess += (msg) =>
             {
                 Console.WriteLine($"Message Received: {msg}");
                 return (++numFailed) >= numToFail;
             };
            receiver.StartReceivingMessages();

            Console.WriteLine($"Queue Length = {await receiver.GetActiveQueueLength()}");

            System.Threading.Thread.Sleep(1000);

            Console.WriteLine($"Queue Length = {await receiver.GetActiveQueueLength()}");

            Console.WriteLine($"DL Length = {await receiver.GetDeadLetterCount()}");

            var dlmsg = await receiver.GetDeadLetterMessage();

            Console.WriteLine($"Dead Letter Received: {dlmsg}");

        }
    }
}
