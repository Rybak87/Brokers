using Brokers.DAL.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Producers
{
    public class RabbitMQProducer : IProducer
    {
        static IModel channel;
        static IConnection conn;
        public void Dispose()
        {
            channel.Close();
            conn.Close();
        }

        public void Initialize()
        {
            var cf = new ConnectionFactory();
            conn = cf.CreateConnection();

            channel = conn.CreateModel();
            channel.QueueDeclare("messages", true, false, false, null);
        }

        public void SendRequest(string message)
        {
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("main", "", null, messageBodyBytes);
        }
    }
}
