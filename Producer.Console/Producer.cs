using Brokers.DAL.Configurations;
using Brokers.DAL.Generator;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using Brokers.DAL.Producers;
using Newtonsoft.Json;
using System;
using System.Configuration;

namespace Producer.Console
{
    public class Producer
    {
        static IProducer producer;
        static readonly IGenerator<Message> generator = new RandomPublicationGenerator();

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
                    producer = new KafkaProducer();
                }
                else
                {
                    RabbitMQSettings settings = (RabbitMQSettings)ConfigurationManager.GetSection("rabbitMQSettings");
                    producer = new RabbitMQProducer(settings);
                }
                producer.Initialize();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                producer?.Dispose();
                System.Console.Read();
                return;
            }

            System.Console.WriteLine("For exit enter \"X\"");
            System.Console.WriteLine("Enter count random messages");


            Sending();
        }

        private static void Sending()
        {
            string input = string.Empty;
            try
            {
                while (input != "X")
                {
                    input = System.Console.ReadLine();

                    if (!int.TryParse(input, out int count))
                    {
                        System.Console.WriteLine("Enter a number");
                    }

                    for (int i = 0; i < count; i++)
                    {
                        var message = generator.GenerateNew();
                        producer.SendRequest(JsonConvert.SerializeObject(message));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                producer.Dispose();
            }
        }
    }
}
