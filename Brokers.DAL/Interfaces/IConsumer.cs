using Brokers.DAL.Model;
using System;

namespace Brokers.DAL.Interfaces
{
    public interface IMessageConsumer
    {
        void StartConsume();
        void Close();
        event Action<Message> NewMessage;
    }
}
