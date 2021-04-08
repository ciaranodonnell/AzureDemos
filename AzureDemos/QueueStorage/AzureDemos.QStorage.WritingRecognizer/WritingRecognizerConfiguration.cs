
namespace AzureDemos.QStorage.WritingRecognizer
{
    public class WritingRecognizerConfiguration
    {
        public string ConnectionString { get; set; }// = Config.GetStringOrError("AZDEMO_QSTORAGECONNECTIONSTRING");
        public string  CognitiveSubscriptionKey { get; set; } //= config.GetStringOrError("AZURE_COGNITIVE_TESTKEY");
        public string QueueName { get; set; } //config.GetStringOrError("AZDEMO_QSTORAGE_WRITINGQUEUE");

    }
}