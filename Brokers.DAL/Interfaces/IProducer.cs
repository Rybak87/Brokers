using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Interfaces
{
    public interface IProducer: IDisposable
    {
        void Initialize();
        void SendRequest(string request);
    }
}
