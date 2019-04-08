using AzureDemos.Common;
using AzureDemos.EventHub.EventProcessor.CommandLine;
using CommandLine;
using System;

namespace AzureDemos.EventHub.EHSender
{
	class Program
	{

		static string connectionString = null;


		static void Main(string[] args)
		{

			try
			{
				connectionString = System.Configuration.ConfigurationManager.AppSettings["asbcs"];
			}
			catch { }
			
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached && args.Length == 0)
			{
				args = $"-c {connectionString}".SplitCommandLineStyle();
			}

#endif



            //Parser.Default.ParseArguments<SendingOptions>(args).WithParsed((options) =>
            //{

            //});





        }
	}
}
