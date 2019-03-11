using AzureDemos.QStorage.WritingRecognizer;
using COD.Platform.Configuration.Basic;
using COD.Platform.Logging.NoOp;
using System;

namespace AzureDemos.QStorage.HoldOldServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new  LayeredConfiguration(
                                new NoOpLoggingService(), new CommandLineConfig(args), new EnvironmentConfiguration(new NoOpLoggingService()));
            
            var connectionString = config.GetStringOrError("AZDEMO_QSTORAGECONNECTIONSTRING");
            var cognitiveSubscriptionKey = config.GetStringOrError("AZURE_COGNITIVE_TESTKEY");
            var queueName = config.GetStringOrError("AZDEMO_QSTORAGE_WRITINGQUEUE");

            var service = new WritingRecognizerService(connectionString, cognitiveSubscriptionKey, queueName);

            service.Run();
            
        }
    }
}
