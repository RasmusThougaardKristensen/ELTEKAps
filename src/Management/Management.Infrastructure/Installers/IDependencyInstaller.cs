using Microsoft.Extensions.DependencyInjection;

namespace ELTEKAps.Management.Infrastructure.Installers;
public interface IDependencyInstaller
{
    void Install(IServiceCollection serviceCollection, DependencyInstallerOptions options);
}