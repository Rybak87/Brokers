using Brokers.DAL.Configurations;
using Brokers.DAL.Consumers;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Loggers;
using Brokers.DAL.Model;
using System;
using System.Configuration;
using System.ServiceProcess;
using Newtonsoft.Json;

namespace Consumer.Service
{
    public partial class ConsumerService : ServiceBase
    {
        private readonly IMessageConsumer consumer;
        private readonly ILogger logger = new Log4Net("loggerLog4net");

        public ConsumerService()
        {
            InitializeComponent();

            try
            {
                var settings = (RabbitMQSettings) ConfigurationManager.GetSection("rabbitMQSettings");
                consumer = new RabbitMQConsumer(settings, logger);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                consumer?.Close();
                throw;
            }
        }

        protected override void OnStart(string[] args)
        {
            consumer.NewMessage += HandlingMessage;
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
    }
}
