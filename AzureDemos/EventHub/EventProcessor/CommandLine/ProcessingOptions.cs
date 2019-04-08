using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDemos.EventHub.EventProcessor.CommandLine
{
    public class ProcessingOptions
    {
        [Option('c', "connectionString", HelpText = "The Connection String for the Event Hub", Required = true)]
        public string ConnectionString { get; set; }


        [Option('t', "topic", HelpText = "The Event Hub topic to publish on", Required = true)]
        public string Topic{ get; set; }


    }
}
