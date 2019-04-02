using AzureDemos.ServiceBus.DemoApp.BusTools;
using AzureDemos.ServiceBus.DemoApp.Verbs;
using CommandLine;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;
using AzureDemos.Common;

namespace AzureDemos.ServiceBus.DemoApp
{
    class Program
    {
        static string connectionString = null;


        static async Task Main(string[] args)
        {

            
            try
            {
                connectionString = System.Configuration.ConfigurationManager.AppSettings["asbcs"];
            }
            catch { }
			
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached && args.Length == 0)
            {
                args = $"{ExpiredMessagesTestVerb.Verb} -c {connectionString}".SplitCommandLineStyle();
            }

#endif




            Parser.Default.ParseArguments<ExpiredMessagesTestVerb, BasicSendReceiveTestVerb>(args)
                .MapResult(
                 (BasicSendReceiveTestVerb opts) => DoBasicTest(opts).Result,
                 (ExpiredMessagesTestVerb opts) => DoExpiredMessagesTest(opts).Result,
                  errs =>
                  {
                      Console.WriteLine("Error Mapping command lines");
                      Console.WriteLine(string.Join(' ', args));
                      return 1;
                  });

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static async Task<int> DoExpiredMessagesTest(ExpiredMessagesTestVerb opts)
        {
            ReceivingExpiredMessages tester = new ReceivingExpiredMessages(opts);
            await tester.RunTest();
            return 0;
        }

        static async Task<int> DoBasicTest(BasicSendReceiveTestVerb opts)
        {
         
            await DoSendAndReceive(opts.ConnectionString, opts.QueuePath);

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
