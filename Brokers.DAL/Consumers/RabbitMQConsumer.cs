using Brokers.DAL.Configurations;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using log4net;
using log4net.Config;
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
        private readonly ILog logger;
        private string queueName;

        public event Action<Message> NewMessage;

        public RabbitMQConsumer(RabbitMQSettings config, ILog logger)
        {
            this.logger = logger;

            try
            {
                InitConnection(config);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw new Exception("Unable connect to RabbitMQ");
            }
        }

        private void InitConnection(RabbitMQSettings config)
        {
            queueName = config.QueueName;

            connection = config.CreateConnection();
            connection.ConnectionShutdown += (o, e) => { logger.Error("Сервер не отвечает"); };
            connection.CallbackException += (o, e) => { logger.Error(e.Exception.Message, e.Exception); };

            channel = connection.CreateModel();
            channel.ExchangeDeclare("main", "fanout", durable: true);
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
            try
            {
                var consumeResult = Encoding.UTF8.GetString(e.Body);
                var message = JsonConvert.DeserializeObject<Message>(consumeResult);
                NewMessage?.Invoke(message);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}
