using ELTEKAps.Management.Infrastructure.Installers;

namespace ELTEKAps.Management.Worker.Service.Installers
{
    public class ServiceInstaller : IDependencyInstaller
    {
        public void Install(IServiceCollection serviceCollection, DependencyInstallerOptions options)
        {
            //Add service dependencies
        }
    }
}