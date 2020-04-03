using System.Collections;

namespace Brokers.DAL.Interfaces
{
    public interface ILogger
    {
        void WriteError(string message);
        ICollection InitLogger();
    }
}
