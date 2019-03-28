using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDemos.ServiceBus.DemoApp.Verbs
{

    [Verb("expiredmsgs", HelpText = "Send messages that will expire on the queue on process them")]
    public class ExpiredMessagesTest
    {

        public ExpiredMessagesTest()
        {

        }

        [Option('c', "connectionString")]
        public string ConnectionString
        { get; set; }



    }
}
