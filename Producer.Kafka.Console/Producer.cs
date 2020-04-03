using Brokers.DAL.Model;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Producer.Kafka.Console
{
    class Producer
    {
        static IProducer<Null, string> producer;
        static Random rand = new Random();
        private const string alphabet = " abcefghijklmnopqrstuvwxyz";

        static void Main(string[] args)
        {
            Initialize();
            string input = string.Empty;

            try
            {
                while (input != "X")
                {
                    System.Console.WriteLine("Enter count random messages");
                    input = System.Console.ReadLine();

                    int count;
                    count = Convert.ToInt32(input);

                    for (int i = 0; i < count; i++)
                    {
                        var message = GetRandomMessage();
                        SendRequest(JsonConvert.SerializeObject(message));
                    }
                }
            }
            catch { }
            finally
            {
                producer.Dispose();
            }
        }

        static void Initialize()
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost",
                ClientId = Dns.GetHostName()
            };
            producer = new ProducerBuilder<Null, string>(config).Build();
        }

        static Message GetRandomMessage()
        {
            return new Message()
            {
                AuthorId = rand.Next(1, 100),
                HostId = rand.Next(1, 100),
                PublicatonId = rand.Next(1, 100),
                ReactionCount = rand.Next(1, 100),
                ViewCount = rand.Next(1, 100),
                Content = new string(Enumerable.Repeat(0, rand.Next(1, 30)).Select(a => alphabet[rand.Next(alphabet.Length)]).ToArray()),
                Title = new string(Enumerable.Repeat(0, rand.Next(1, 30)).Select(a => alphabet[rand.Next(alphabet.Length)]).ToArray())
            };
        }

        static void SendRequest(string message)
        {
            producer.ProduceAsync("main", new Message<Null, string> { Value = message });
        }
    }
}
