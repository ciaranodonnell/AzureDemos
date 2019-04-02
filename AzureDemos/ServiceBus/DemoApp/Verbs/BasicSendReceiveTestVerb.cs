using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDemos.ServiceBus.DemoApp.Verbs
{
    [Verb("basic")]
    class BasicSendReceiveTestVerb
    {

        public BasicSendReceiveTestVerb()
        {
            try
            {
                this.QueuePath = System.Configuration.ConfigurationManager.AppSettings["demoqueue"];
            }
            catch { }


            try
            {
                this.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["asbcs"];
            }
            catch { }

        }

        [Option('c', "connectionString")]
        public string ConnectionString
        { get; set; }


        [Option('q', "queue", Required = false, HelpText = "The name of the queue for the test. If left blank a random queue name will be generated and created")]
        public string QueuePath { get; set; } = "basictestqueue-" + new Random().Next(int.MaxValue);

        [Option('n', "nummsgs", Default = 10)]
        public int NumberOfMessages { get; set; }

        [Option('f', "numfails", Default = 0)]
        public int NumberOfProcessingFailures { get; set; }

    }
}
