using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using Confluent.Kafka;
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

        private readonly ILogger logger;

        public void Close()
        {
            cancelTokenSource.Cancel();
        }

        public void StartConsume()
        {
            token = cancelTokenSource.Token;
            Receive(token);
        }

        public KafkaConsumer()
        {
            Initialize();
        }

        private void Initialize()
        {
            config = new ConsumerConfig()
            {
                BootstrapServers = "localhost",
                GroupId = "abc",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoOffsetStore = false
            };
        }
        private async Task Receive(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumer.Subscribe("main");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        var strMessage = consumeResult.Message.Value;
                        Message message;
                        try
                        {
                            message = JsonConvert.DeserializeObject<Message>(strMessage);
                        }
                        catch (Exception ex)
                        {
                            logger.WriteError(ex.Message);
                            return;
                        }
                        NewMessage?.BeginInvoke(message, null, null);
                        consumer.Commit(consumeResult);
                    }
                    consumer.Close();
                }
            });
        }
    }
}
