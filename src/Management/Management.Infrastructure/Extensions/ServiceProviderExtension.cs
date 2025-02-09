using ELTEKAps.Management.Infrastructure.Installers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ELTEKAps.Management.Infrastructure.Extensions;
public static class ServiceProviderExtension
{
    public static void AddFromAssembly<TAssemblyType>(this IServiceCollection self, DependencyInstallerOptions options)
    {
        if (self == null)
            throw new ArgumentNullException(nameof(self));

        var assemblyToRegister = GetAssembly<TAssemblyType>();

        RegisterAssembly(self, assemblyToRegister, options);
    }

    private static Assembly GetAssembly<THandler>()
    {
        return typeof(THandler).Assembly;
    }

    private static void RegisterAssembly(IServiceCollection services, Assembly assemblyToRegister, DependencyInstallerOptions options)
    {
        var typesToAutoRegister = assemblyToRegister.GetTypes()
            .Where(IsDependencyInstaller)
            .ToList();

        foreach (var type in typesToAutoRegister)
        {
            var instance = Activator.CreateInstance(type);
            var typedInstance = instance as IDependencyInstaller;
            typedInstance?.Install(services, options);
        }
    }

    private static bool IsDependencyInstaller(Type type)
    {
        return !type.IsInterface
               && !type.IsAbstract
               && typeof(IDependencyInstaller).IsAssignableFrom(type);
    }
}
