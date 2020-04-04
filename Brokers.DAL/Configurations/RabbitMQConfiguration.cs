using RabbitMQ.Client;
using System;
using System.Configuration;

namespace Brokers.DAL.Configurations
{
    public class RabbitMQSettings : ConfigurationSection
    {

        [ConfigurationProperty("automaticRecoveryEnabled")]
        public bool AutomaticRecoveryEnabled
        {
            get { return (bool)base["automaticRecoveryEnabled"]; }
        }

        [ConfigurationProperty("continuationTimeout")]
        public TimeSpan ContinuationTimeout
        {
            get { return (TimeSpan)base["continuationTimeout"]; }
        }

        [ConfigurationProperty("dispatchConsumersAsync")]
        public bool DispatchConsumersAsync
        {
            get { return (bool)base["dispatchConsumersAsync"]; }
        }

        [ConfigurationProperty("handshakeContinuationTimeout")]
        public TimeSpan HandshakeContinuationTimeout
        {
            get { return (TimeSpan)base["handshakeContinuationTimeout"]; }
        }

        [ConfigurationProperty("hostName")]
        public string HostName
        {
            get { return (string)base["hostName"]; }
        }

        [ConfigurationProperty("networkRecoveryInterval")]
        public TimeSpan NetworkRecoveryInterval
        {
            get { return (TimeSpan)base["networkRecoveryInterval"]; }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)base["password"]; }
        }

        [ConfigurationProperty("port")]
        public int Port
        {
            get { return (int)base["port"]; }
        }

        [ConfigurationProperty("queueName")]
        public string QueueName
        {
            get { return (string)base["queueName"]; }
        }

        [ConfigurationProperty("requestedChannelMax")]
        public ushort RequestedChannelMax
        {
            get { return (ushort)base["requestedChannelMax"]; }
        }

        [ConfigurationProperty("requestedConnectionTimeout")]
        public int RequestedConnectionTimeout
        {
            get { return (int)base["requestedConnectionTimeout"]; }
        }

        [ConfigurationProperty("requestedFrameMax")]
        public uint RequestedFrameMax
        {
            get { return (uint)base["requestedFrameMax"]; }
        }

        [ConfigurationProperty("requestedHeartbeat")]
        public ushort RequestedHeartbeat
        {
            get { return (ushort)base["requestedHeartbeat"]; }
        }

        [ConfigurationProperty("socketReadTimeout")]
        public int SocketReadTimeout
        {
            get { return (int)base["socketReadTimeout"]; }
        }

        [ConfigurationProperty("topologyRecoveryEnabled")]
        public bool TopologyRecoveryEnabled
        {
            get { return (bool)base["topologyRecoveryEnabled"]; }
        }

        [ConfigurationProperty("useBackgroundThreadsForIO")]
        public bool UseBackgroundThreadsForIO
        {
            get { return (bool)base["useBackgroundThreadsForIO"]; }
        }

        [ConfigurationProperty("userName")]
        public string UserName
        {
            get { return (string)base["userName"]; }
        }

        [ConfigurationProperty("virtualHost")]
        public string VirtualHost
        {
            get { return (string)base["virtualHost"]; }
        }

        public IConnection CreateConnection() => GetConnectionFactory().CreateConnection();

        public ConnectionFactory GetConnectionFactory()
        {
            var cf = new ConnectionFactory();
            cf.AutomaticRecoveryEnabled = AutomaticRecoveryEnabled;
            cf.ContinuationTimeout = ContinuationTimeout;
            cf.DispatchConsumersAsync = DispatchConsumersAsync;
            cf.HandshakeContinuationTimeout = HandshakeContinuationTimeout;
            cf.HostName = HostName;
            cf.NetworkRecoveryInterval = NetworkRecoveryInterval;
            cf.Password = Password;
            cf.Port = Port;
            cf.RequestedChannelMax = RequestedChannelMax;
            cf.RequestedConnectionTimeout = RequestedConnectionTimeout;
            cf.RequestedFrameMax = RequestedFrameMax;
            cf.RequestedHeartbeat = RequestedHeartbeat;
            cf.SocketReadTimeout = SocketReadTimeout;
            cf.TopologyRecoveryEnabled = TopologyRecoveryEnabled;
            cf.UseBackgroundThreadsForIO = UseBackgroundThreadsForIO;
            cf.UserName = UserName;
            cf.VirtualHost = VirtualHost;

            return cf;
        }
    }
}
