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
                var cf = GetConnectionFactory(config);

                connection = cf.CreateConnection();
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

        private ConnectionFactory GetConnectionFactory(RabbitMQSettings config)
        {
            var cf = new ConnectionFactory();
            cf.AutomaticRecoveryEnabled = config.AutomaticRecoveryEnabled;
            cf.ContinuationTimeout = config.ContinuationTimeout;
            cf.DispatchConsumersAsync = config.DispatchConsumersAsync;
            cf.HandshakeContinuationTimeout = config.HandshakeContinuationTimeout;
            cf.HostName = config.HostName;
            cf.NetworkRecoveryInterval = config.NetworkRecoveryInterval;
            cf.Password = config.Password;
            cf.Port = config.Port;
            cf.RequestedChannelMax = config.RequestedChannelMax;
            cf.RequestedConnectionTimeout = config.RequestedConnectionTimeout;
            cf.RequestedFrameMax = config.RequestedFrameMax;
            cf.RequestedHeartbeat = config.RequestedHeartbeat;
            cf.SocketReadTimeout = config.SocketReadTimeout;
            cf.TopologyRecoveryEnabled = config.TopologyRecoveryEnabled;
            cf.UseBackgroundThreadsForIO = config.UseBackgroundThreadsForIO;
            cf.UserName = config.UserName;
            cf.VirtualHost = config.VirtualHost;

            return cf;
        }
    }
}
