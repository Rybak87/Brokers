using Brokers.DAL.Configurations;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Brokers.DAL.Consumers
{

    public class RabbitMQConsumer : IMessageConsumer
    {
        private IConnection connection;
        private IModel channel;
        private EventingBasicConsumer consumer;
        private readonly ILogger logger;
        private string queueName;

        public event Action<Message> NewMessage;

        public RabbitMQConsumer(RabbitMQSettings config, ILogger logger)
        {
            this.logger = logger;
            this.logger.InitLogger();

            try
            {
                InitConnection(config);
            }
            catch (Exception ex)
            {
                logger.WriteError(ex.Message);
                throw new Exception("Unable connect to RabbitMQ");
            }
        }

        private void InitConnection(RabbitMQSettings config)
        {
            queueName = config.QueueName;

            connection = config.CreateConnection();
            connection.ConnectionShutdown += (o, e) => { logger.WriteError("Сервер не отвечает"); };
            connection.CallbackException += (o, e) => { logger.WriteError(e.Exception.Message); };

            channel = connection.CreateModel();
            channel.ExchangeDeclare("main", "fanout");
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, "main", "", null);

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Receive;
        }

        public void StartConsume()
        {
            channel.BasicConsume(queueName, true, consumer);
        }

        public void Close()
        {
            channel?.Close();
            connection?.Close();
        }

        private void Receive(object sender, BasicDeliverEventArgs e)
        {
            Message message;
            try
            {
                var consumeResult = Encoding.UTF8.GetString(e.Body);
                message = JsonConvert.DeserializeObject<Message>(consumeResult);
            }
            catch (Exception ex)
            {
                logger.WriteError(ex.Message);
                return;
            }
            NewMessage?.BeginInvoke(message, null, null);
        }
    }
}
