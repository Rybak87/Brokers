using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using Confluent.Kafka;
using log4net;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brokers.DAL.Consumers
{
    public class KafkaConsumer : IMessageConsumer
    {
        public event Action<Message> NewMessage;
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        CancellationToken token;
        ConsumerConfig config;
        private readonly ILog logger;

        public KafkaConsumer(ILog logger)
        {
            Initialize();
            this.logger = logger;
        }

        public void Close()
        {
            cancelTokenSource.Cancel();
        }

        public void StartConsume()
        {
            token = cancelTokenSource.Token;
            Receive(token);
        }

        private void Initialize()
        {
            try
            {
                config = new ConsumerConfig()
                {
                    BootstrapServers = "localhost",
                    GroupId = "abc",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                };
            }
            catch
            {
                throw new Exception("Unable connect to RabbitMQ");
            }
        }
        private async Task Receive(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    Message message;
                    consumer.Subscribe("main");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        var strMessage = consumeResult.Message.Value;
                        try
                        {
                            message = JsonConvert.DeserializeObject<Message>(consumeResult.Message.Value);
                            NewMessage?.BeginInvoke(message, null, null);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message, ex);
                        }
                    }
                    consumer.Close();
                }
            });
        }
    }
}
