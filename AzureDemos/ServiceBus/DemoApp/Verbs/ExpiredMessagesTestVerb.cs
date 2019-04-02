using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDemos.ServiceBus.DemoApp.Verbs
{

    [Verb(ExpiredMessagesTestVerb.Verb, HelpText = "Send messages that will expire on the queue on process them")]
    public class ExpiredMessagesTestVerb
    {
        public const string Verb = "expiredmsgs";

        public ExpiredMessagesTestVerb()
        {
            try
            {
                var value = System.Configuration.ConfigurationManager.AppSettings["demoqueue"];
                if (value != null) this.QueuePath = value;
            }
            catch { }


            try
            {

                var value = System.Configuration.ConfigurationManager.AppSettings["asbcs"];
                if (value != null) this.ConnectionString = value;
            }
            catch { }

        }


        [Option('c', "connectionString")]
        public string ConnectionString
        { get; set; }



        [Option('q', "queue", Required = false, HelpText = "The name of the queue for the test. If left blank a random queue name will be generated and created")]
        public string QueuePath { get; set; } = "expiredmsgtestqueue-" + new Random().Next(int.MaxValue);

    }
}
