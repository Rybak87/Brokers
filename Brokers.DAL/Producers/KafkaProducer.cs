using Brokers.DAL.Interfaces;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Producers
{
    public class KafkaProducer : IProducer
    {
        static IProducer<Null, string> producer;
        public void Dispose()
        {
            producer.Dispose();
        }

        public void Initialize()
        {
            try
            {
                var config = new ProducerConfig()
                {
                    BootstrapServers = "localhost",
                    ClientId = Dns.GetHostName()
                };
                producer = new ProducerBuilder<Null, string>(config).Build();
            }
            catch
            {
                throw new Exception("Unable connect to RabbitMQ");
            }
        }

        public void SendRequest(string message)
        {
            producer.ProduceAsync("main", new Message<Null, string> { Value = message });
        }
    }
}
