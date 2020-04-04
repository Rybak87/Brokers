using System.ServiceProcess;

namespace Consumer.Service
{
    static class MainService
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ConsumerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
