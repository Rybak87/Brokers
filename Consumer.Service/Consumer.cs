using Brokers.DAL.Configurations;
using Brokers.DAL.Consumers;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using System;
using System.Configuration;
using System.ServiceProcess;
using Newtonsoft.Json;
using Nest;
using log4net.Config;
using log4net;

namespace Consumer.Service
{
    public partial class ConsumerService : ServiceBase
    {
        private readonly IMessageConsumer consumer;
        static ILog logger = LogManager.GetLogger("loggerLog4net");
        static readonly string nameIndex = "messages";
        static ElasticClient esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));

        public ConsumerService()
        {
            InitializeComponent();
            XmlConfigurator.Configure();

            try
            {
                var settings = (RabbitMQSettings) ConfigurationManager.GetSection("rabbitMQSettings");
                consumer = new RabbitMQConsumer(settings, logger);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                consumer?.Close();
                throw;
            }
        }

        protected override void OnStart(string[] args)
        {
            consumer.NewMessage += HandlingMessage;
            consumer.NewMessage += ResendMessageToES;
            consumer.StartConsume();
        }

        protected override void OnStop()
        {
            consumer.Close();
        }

        private void HandlingMessage(Message message)
        {
            logger.Debug(JsonConvert.SerializeObject(message));
        }

        private static void ResendMessageToES(Message message)
        {
            _ = esClient.Index(message, i => i.Id(message.PublicatonId));
        }
    }
}
