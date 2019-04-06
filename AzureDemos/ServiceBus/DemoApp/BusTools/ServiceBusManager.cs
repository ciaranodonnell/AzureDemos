using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureDemos.ServiceBus.DemoApp.BusTools
{
    class ServiceBusManager
    {
        private string connectionString;
        private ManagementClient mgtClient;

        public ServiceBusManager(string serviceBusConnectionString)
        {
            this.connectionString = serviceBusConnectionString;
            mgtClient = new ManagementClient(serviceBusConnectionString);
        }

        #region Queue Management

        public async Task CreateQueueAsync(string queuePath)
        {
            await CreateQueueAsync(new QueueDescription(queuePath), false);
        }
        public async Task CreateQueueAsync(string queuePath, TimeSpan messageTTL, bool enableDeadLetteringOnMessageExpiration, bool deleteAndRemakeIfExists)
        {
            await CreateQueueAsync(new QueueDescription(queuePath)
            {
                DefaultMessageTimeToLive = messageTTL,
                EnableDeadLetteringOnMessageExpiration = enableDeadLetteringOnMessageExpiration
            }, deleteAndRemakeIfExists);
        }

        public async Task CreateQueueAsync(QueueDescription description, bool deleteAndRemakeIfExists)
        {
            var exists = await mgtClient.QueueExistsAsync(description.Path);

            if (exists && deleteAndRemakeIfExists)
            {
                await mgtClient.DeleteQueueAsync(description.Path);
                exists = false;
            }

            if (!exists)
            {
                await mgtClient.CreateQueueAsync(description);
            }

        }

        internal async Task DeleteQueueAsync(string queuePath)
        {
            var exists = await mgtClient.QueueExistsAsync(queuePath);
            if (exists)
                await mgtClient.DeleteQueueAsync(queuePath);

        }


        #endregion

    }
}
