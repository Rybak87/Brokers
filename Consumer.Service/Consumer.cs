using Brokers.DAL.Configurations;
using Brokers.DAL.Consumers;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Loggers;
using Brokers.DAL.Model;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace Consumer.Service
{
    public partial class ConsumerService : ServiceBase
    {
        static IMessageConsumer consumer;
        static ILogger logger = new Log4Net("loggerLog4net");
        static object locker = new object();

        public ConsumerService()
        {
            InitializeComponent();

            try
            {
                var settings = (RabbitMQSettings)ConfigurationManager.GetSection("rabbitMQSettings");
                consumer = new RabbitMQConsumer(settings, logger);
                //consumer = new KafkaConsumer(logger);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
                return;
            }

            consumer.StartConsume();
            //consumer.NewMessage += HandlingMessage;

            Console.Read();
            consumer.Close();
        }

        protected override void OnStart(string[] args)
        {
            consumer.StartConsume();
        }

        protected override void OnStop()
        {
            consumer.Close();
        }

        private static void HandlingMessage(Message message)
        {
            var props = typeof(Message).GetProperties().Select(p => new { name = p.Name, value = p.GetValue(message) });

            lock (locker)
            {
                using (var sw = new StreamWriter("C:\\Consumer\\Messages.txt"))
                {

                    foreach (var item in props)
                    {
                        sw.WriteLine(string.Format($"{item.name} = {item.value}"));
                        sw.WriteLine();
                    }
                }
            }
        }
    }
}
