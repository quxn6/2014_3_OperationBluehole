using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Server
{
    using RabbitMQ.Client;
    using System.Text;
    using LitJson;

    static class MessageManager
    {
        public readonly static RabbitMQ.Client.ConnectionFactory connectionFactory;

        static MessageManager()
        {
            connectionFactory = new ConnectionFactory() { HostName = Config.MATCHING_QUEUE_ADDRESS };
        }
    }
}
