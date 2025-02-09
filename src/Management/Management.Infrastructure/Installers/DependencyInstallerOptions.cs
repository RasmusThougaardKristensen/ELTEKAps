using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ELTEKAps.Management.Infrastructure.Installers;
/// <summary>
///     This class contains properties and configuration for installing dependencies
/// </summary>
public class DependencyInstallerOptions
{
    public IConfiguration Configuration { get; protected set; }
    public IHostEnvironment HostEnvironment { get; protected set; }

    public DependencyInstallerOptions(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
    }
}
