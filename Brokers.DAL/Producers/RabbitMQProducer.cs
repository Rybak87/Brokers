using Brokers.DAL.Configurations;
using Brokers.DAL.Interfaces;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Brokers.DAL.Producers
{
    public class RabbitMQProducer : IProducer
    {
        static IModel channel;
        static IConnection connection;
        RabbitMQSettings config;

        public RabbitMQProducer(RabbitMQSettings config)
        {
            this.config = config;
        }
        public void Dispose()
        {
            channel?.Close();
            connection?.Close();
        }

        public void Initialize()
        {
            try
            {
                connection = config.CreateConnection();
                channel = connection.CreateModel();
                channel.QueueDeclare("messages", true, false, false, null);

            }
            catch
            {
                throw new Exception("Unable connect to RabbitMQ");
            }
        }

        public void SendRequest(string message)
        {
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("main", "", null, messageBodyBytes);
        }
    }
}
