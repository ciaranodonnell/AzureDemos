using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDemos.QStorage.WritingRecognizer
{
    public class WritingRecognizerService
    {
        private string queueStorageConnectionString;
        private string cognitiveSubscriptionKey;
        private string queueName;
        private CloudStorageAccount storageAccount;
        private CloudQueue queue;

        public WritingRecognizerService(string connectionString, string cognitiveSubscriptionKey, string queueName = "WritingRecognizerQueue")
        {
            this.queueStorageConnectionString = connectionString;
            this.cognitiveSubscriptionKey = cognitiveSubscriptionKey;
            this.queueName = queueName.ToLowerInvariant();
        }

        public void Run()
        {
            this.storageAccount = CloudStorageAccount.Parse(queueStorageConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            this.queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();


            while (true)
            {
                var message = this.queue.GetMessage(TimeSpan.FromSeconds(10));
                if(message == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                var msgbytes = message.AsBytes;
                var request = DeserializeRequest(msgbytes);

                Thread.Sleep(8000);

                //uh ok - we've used almost all our 10 seconds. better ask for more time
                queue.UpdateMessage(message, TimeSpan.FromSeconds(20), MessageUpdateFields.Visibility);

                //System.IO.File.WriteAllBytes(request.Image)
                
                var result = MakeAnalysisRequest(request.Image).GetAwaiter().GetResult();

                Console.WriteLine(result);

                queue.DeleteMessage(message);

            }

        }

        private IWritingRecognizerRequest DeserializeRequest(byte[] msgBytes)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(WritingRecognizerRequest));
            return serializer.ReadObject(new System.IO.MemoryStream(msgBytes)) as WritingRecognizerRequest;
        }

        // You must use the same Azure region in your REST API method as you used to
        // get your subscription keys. For example, if you got your subscription keys
        // from the West US region, replace "westcentralus" in the URL
        // below with "westus".
        //
        // Free trial subscription keys are generated in the "westus" region.
        // If you use a free trial subscription key, you shouldn't need to change
        // this region.
        const string uriBase =
          "https://eastus.api.cognitive.microsoft.com/vision/v2.0/read/core/asyncBatchAnalyze";


        async Task<string> MakeAnalysisRequest(byte[] image)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", cognitiveSubscriptionKey);

                // Request parameters. A third optional parameter is "details".
                // The Analyze Image method returns information about the following
                // visual features:
                // Categories:  categorizes image content according to a
                //              taxonomy defined in documentation.
                // Description: describes the image content with a complete
                //              sentence in supported languages.
                // Color:       determines the accent color, dominant color, 
                //              and whether an image is black & white.
                string requestParameters = "mode=Handwritten";


                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(image))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                // operationLocation stores the URI of the second REST API method,
                // returned by the first REST API method.
                string operationLocation;


                // The response header for the Batch Read method contains the URI
                // of the second method, Read Operation Result, which
                // returns the results of the process in the response body.
                // The Batch Read operation does not return anything in the response body.
                if (response.IsSuccessStatusCode)
                    operationLocation =
                        response.Headers.GetValues("Operation-Location").FirstOrDefault();
                else
                {
                    // Display the JSON error data.
                    string errorString = await response.Content.ReadAsStringAsync();

                    return JToken.Parse(errorString).ToString();
                }

                // If the first REST API method completes successfully, the second 
                // REST API method retrieves the text written in the image.
                //
                // Note: The response may not be immediately available. Handwriting
                // recognition is an asynchronous operation that can take a variable
                // amount of time depending on the length of the handwritten text.
                // You may need to wait or retry this operation.
                //
                // This example checks once per second for ten seconds.
                string contentString;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++i;
                }
                while (i < 100 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

                if (i == 100 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
                {
                    return "Timeout error";
                }

                // Display the JSON response.
                return JToken.Parse(contentString).ToString();
            }
            catch (Exception e)
            {
               return e.Message;
            }
        }


    }
}
