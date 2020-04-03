using Brokers.DAL.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace Publisher.Console
{

    class Publisher
    {
        private const string alphabet = "abcefghijklmnopqrstuvwxys";
        static IModel channel;

        static void Main(string[] args)
        {
            var cf = new ConnectionFactory();
            var conn = cf.CreateConnection();

            channel = conn.CreateModel();
            channel.QueueDeclare("messages", true, false, false, null);

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
            finally
            {
                channel.Close();
                conn.Close();
            }
        }

        static Message GetRandomMessage()
        {
            var rand = new Random();
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
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("main", "", null, messageBodyBytes);
        }
    }
}
