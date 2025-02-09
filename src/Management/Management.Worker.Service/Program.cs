using ELTEKAps.Management.Infrastructure.Extensions;
using ELTEKAps.Management.Infrastructure.Installers;
using ELTEKAps.Management.Infrastructure.Startup;
using Microsoft.AspNetCore;

namespace ELTEKAps.Management.Worker.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateWebHostBuilder(args).Build();

        var runOnStartupExecution = host.Services.GetRequiredService<IRunOnStartupExecution>();

        await runOnStartupExecution.RunAll();
        await host.RunAsync();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, builder) =>
               {
                   builder
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                       .AddEnvironmentVariables()
                       .Build();
               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                   logging.AddConsole();
               })
               .ConfigureServices((context, collection) =>
               {
                   var configuration = context.Configuration;

                   var parameters = new DependencyInstallerOptions(configuration, context.HostingEnvironment);
                   collection.AddFromAssembly<Program>(parameters);
                   collection.AddFromAssembly<ServiceInstaller>(parameters);
               })
               .UseStartup<Startup>();
}
