using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace AzureDemos.QStorage.WritingRecognizer
{
    public class WritingRecognizerClient
    {
        private string connectionString;
        private string queueName;
        private CloudStorageAccount storageAccount;
        private CloudQueue queue;

        public WritingRecognizerClient(string queueStorageConnectionString, string queueName = "writingrecognizerqueue")
        {
            this.connectionString = queueStorageConnectionString;
            this.queueName = queueName.ToLowerInvariant();

            this.storageAccount = CloudStorageAccount.Parse(queueStorageConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            this.queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();


        }

        public async Task SendMessageAsync(IWritingRecognizerRequest request)
        {
            var bytes = SerializeRequest(request);


            await queue.AddMessageAsync(new CloudQueueMessage(bytes));


        }

        private byte[] SerializeRequest(IWritingRecognizerRequest request)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(WritingRecognizerRequest));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, request);
            return ms.ToArray();
        }
    }
}
