using Brokers.DAL.Interfaces;
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
    public class ProducerRandom<T> where T : class, new()
    {
        static Random rand = new Random();
        private const string alphabet = " abcefghijklmnopqrstuvwxyz";
        static IProducer producer = new RabbitMQProducer();
        static List<PropertyInfo> props;

        static void Main(string[] args)
        {
            var props = typeof(T).GetProperties().Where(p => p.CanWrite
                && p.PropertyType == typeof(int)
                || p.PropertyType == typeof(string)).ToList();

            producer.Initialize();
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
                        var message = GetRandomEntity();
                        producer.SendRequest(JsonConvert.SerializeObject(message));
                    }
                }
            }
            catch { }
            finally
            {
                producer.Dispose();
            }
        }

        static T GetRandomEntity()
        {
            var ent = new T();
            props.ForEach(p =>
            {
                if (p.PropertyType == typeof(int))
                {
                    p.SetValue(ent, rand.Next(1, 100));
                }
                else if (p.PropertyType == typeof(string))
                {
                    var genString = new string(Enumerable.Repeat(0, rand.Next(1, 30)).Select(a => alphabet[rand.Next(alphabet.Length)]).ToArray());
                    p.SetValue(ent, genString);
                }
            });
            return ent;
        }
    }
}
