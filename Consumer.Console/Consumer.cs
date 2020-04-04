using Brokers.DAL.Configurations;
using Brokers.DAL.Consumers;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Loggers;
using Brokers.DAL.Model;
using System;
using System.Configuration;
using System.Linq;

namespace Brokers.DAL.Console
{
    class Consumer
    {
        static IMessageConsumer consumer;
        static ILogger logger = new Log4Net("loggerLog4net");
        static object locker = new object();


        static void Main(string[] args)
        {
            System.Console.WriteLine("Select a broker: \"R\" - RabbitMQ, \"K\" - Kafka");
            string choice = System.Console.ReadLine();
            while (choice != "R" && choice != "K")
            {
                System.Console.WriteLine("Try again");
                choice = System.Console.ReadLine();
            }

            try
            {
                if (choice == "K")
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

            consumer.NewMessage += HandlingMessage;// (работает)
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
    }
}
