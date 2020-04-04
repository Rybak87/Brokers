using Brokers.DAL.Configurations;
using Brokers.DAL.Generator;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using Brokers.DAL.Producers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Console
{
    public class Producer
    {
        static IProducer producer;
        static IGenerator<Message> generator = new SimpleGenerator<Message>();

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
                    //TODO: KafkaProducer
                    //producer = new KafkaProducer();
                }
                else
                {
                    RabbitMQSettings settings = (RabbitMQSettings)ConfigurationManager.GetSection("rabbitMQSettings");
                    producer = new RabbitMQProducer(settings);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.Read();
                return;
            }

                                 
            producer.Initialize();
            string input = string.Empty;
            System.Console.WriteLine("Enter count random messages");
            
            try
            {
                while (input != "X")
                {
                    input = System.Console.ReadLine();
                    int count = 0;
                    try
                    {
                        count = Convert.ToInt32(input);
                    }
                    catch
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
            catch(Exception ex)
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
