using Brokers.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Producers
{
    public class RabbitMQProducer : IProducer
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void SendRequest(string request)
        {
            throw new NotImplementedException();
        }
    }
}
