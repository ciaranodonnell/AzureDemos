using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDemos.ServiceBus.DemoApp.Verbs
{
    [Verb(CompetingConsumerTestVerb.Verb, HelpText = "Send messages that will expire on the queue on process them")]
    public class CompetingConsumerTestVerb
    {

        public const string Verb = "compete";

        public CompetingConsumerTestVerb()
        {
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



        [Option('t', "type")]
        public string TopicOrQueue
        { get; set; }





    }
}
