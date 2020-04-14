using Brokers.DAL.Configurations;
using Brokers.DAL.Consumers;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using log4net;
using log4net.Config;
using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;

namespace Consumer.Console
{
    public class Consumer
    {
        static IMessageConsumer consumer;
        static ILog logger = LogManager.GetLogger("loggerLog4net");
        static readonly string nameIndex = "messages";
        static ElasticClient esClient;
        static long counterLogMessagesValid = 0;
        static long counterLogMessagesInvalid = 0;
        static Timer timerLogMessage = new Timer(60000);

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            try
            {
                esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));
            }
            catch (Exception ex)
            {
                logger.Error("Unable create ElasticClient", ex);
            }


            timerLogMessage.Elapsed += (s, e) =>
            {
                logger.Info(string.Format("Count of indexing messages: {0}", counterLogMessagesValid));
                logger.Info(string.Format("Count of indexing errors: {0}", counterLogMessagesInvalid));
                counterLogMessagesValid = 0;
                counterLogMessagesInvalid = 0;
            };

            try
            {
                if (args.Select(s => s.ToLower()).Contains("kafka"))
                {
                    //TODO: KafkaSettings
                    consumer = new KafkaConsumer(logger);
                }
                else
                {
                    RabbitMQSettings settings = (RabbitMQSettings)ConfigurationManager.GetSection("rabbitMQSettings");
                    consumer = new RabbitMQConsumer(settings, logger);
                }
                logger.Info("Consumer created");
            }
            catch (Exception ex)
            {
                logger.Error("Unable create consumer", ex);
            }

            consumer.NewMessage += ResendMessageToES;
            timerLogMessage.Start();


            try
            {
                consumer.StartConsume();
                logger.Info("Started consume");
            }
            catch (Exception ex)
            {
                logger.Error("Unable start consume", ex);
            }

            System.Console.Read();
            consumer.Close();
        }

        static void ResendMessageToES(Message message)
        {
            try
            {
                var result = esClient.Index(message, i => i.Id(message.PublicatonId));
                logger.Debug(string.Format("Indexing message id = {0}", message.PublicatonId), result.OriginalException);

                if (result.IsValid)
                {
                    counterLogMessagesValid++;
                }
                else
                {
                    counterLogMessagesInvalid++;
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Unable to send a message", ex);
            }
        }
    }
}
