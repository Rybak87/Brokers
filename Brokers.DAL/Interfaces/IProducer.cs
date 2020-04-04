using System;

namespace Brokers.DAL.Interfaces
{
    public interface IProducer : IDisposable
    {
        void Initialize();
        void SendRequest(string message);
    }
}
