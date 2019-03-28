using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace AzureDemos.ServiceBus.DemoApp
{
    class ReceivingExpiredMessages
    {
        private string connectionString;
        private QueueClient client;
        private ManagementClient mgt;
        private string queueName;

        public ReceivingExpiredMessages(string serviceBusConnectionString, string queueName)
        {
            this.connectionString = serviceBusConnectionString;

            this.client = new QueueClient(serviceBusConnectionString, queueName);

            mgt = new ManagementClient(serviceBusConnectionString);

            this.queueName = queueName;
        }
    }
}
