using Brokers.DAL.Interfaces;
using log4net;
using log4net.Config;
using System.Collections;

namespace Brokers.DAL.Loggers
{
    public class Log4Net : ILogger
    {
        public Log4Net(string LoggerName)
        {
            Log = LogManager.GetLogger(LoggerName);
        }

        public ILog Log { get; private set; }

        public ICollection InitLogger()
        {
            return XmlConfigurator.Configure();
        }

        public void Error(string message)
        {
            Log.Error(message);
        }

        public void Debug(string message)
        {
            Log.Debug(message);
        }
    }
}
