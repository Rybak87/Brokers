using Brokers.DAL.Generator;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using Brokers.DAL.Producers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Console
{
    public class Producer
    {
        static IProducer producer = new RabbitMQProducer();
        static IGenerator<Message> generator = new SimpleGenerator<Message>();

        static void Main(string[] args)
        {
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
