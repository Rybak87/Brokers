using System.Collections;

namespace Brokers.DAL.Interfaces
{
    public interface ILogger
    {
        void Error(string message);
        
        void Debug(string message);

        ICollection InitLogger();
    }
}
