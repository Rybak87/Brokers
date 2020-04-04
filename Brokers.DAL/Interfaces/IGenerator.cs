using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Interfaces
{
    public interface IGenerator<T> where T:class, new()
    {
        T GenerateNew();
    }
}
