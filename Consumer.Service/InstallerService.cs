using System.ComponentModel;
using System.ServiceProcess;

namespace Consumer.Service
{
    [RunInstaller(true)]
    public partial class InstallerService : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public InstallerService()
        {
            InitializeComponent();

            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "ConsumerService";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
