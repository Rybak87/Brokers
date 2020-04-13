﻿using Brokers.DAL.Configurations;
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

namespace Consumer.Console
{
    public class Consumer
    {
        static IMessageConsumer consumer;
        static ILog logger = LogManager.GetLogger("loggerLog4net");
        static object locker = new object();
        static readonly string nameIndex = "messages";
        static ElasticClient esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

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
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.Read();
                return;
            }

            //consumer.NewMessage += HandlingMessage;
            consumer.NewMessage += ResendMessageToES;
            consumer.StartConsume();

            System.Console.Read();
            consumer.Close();
        }

        private static void HandlingMessage(Message message)
        {
            var props = typeof(Message).GetProperties().Select(p => new { name = p.Name, value = p.GetValue(message) });
            lock (locker)
            {
                foreach (var item in props)
                {
                    System.Console.WriteLine(string.Format($"{item.name} = {item.value}"));
                }
                System.Console.WriteLine();
            }
        }

        private static void ResendMessageToES(Message message)
        {
            try
            {
                var result = esClient.Index(message, i => i.Id(message.PublicatonId));
                //Проверка на результат
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}
