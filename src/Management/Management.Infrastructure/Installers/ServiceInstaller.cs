using Azure.Storage.Blobs;
using ELTEKAps.Management.ApplicationServices.Components;
using ELTEKAps.Management.ApplicationServices.Repositories.Comments;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.ApplicationServices.Repositories.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Photos;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.Infrastructure.Components;
using ELTEKAps.Management.Infrastructure.Repositories.Comments;
using ELTEKAps.Management.Infrastructure.Repositories.Customers;
using ELTEKAps.Management.Infrastructure.Repositories.Operations;
using ELTEKAps.Management.Infrastructure.Repositories.Photos;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using ELTEKAps.Management.Infrastructure.Repositories.Users;
using ELTEKAps.Management.Infrastructure.Startup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELTEKAps.Management.Infrastructure.Installers;

public sealed class ServiceInstaller : IDependencyInstaller
{
    public void Install(IServiceCollection serviceCollection, DependencyInstallerOptions options)
    {
        serviceCollection.AddTransient<IRunOnStartupExecution, RunOnStartupExecution>();
        AddRepositories(serviceCollection, options.Configuration);
        AddComponents(serviceCollection, options.Configuration);
    }

    private static void AddRepositories(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration[Constants.ConfigurationKeys.SqlDbConnectionString];

        serviceCollection.AddDbContext<TaskContext>(options => options.UseSqlServer(connectionString));

        serviceCollection.AddScoped<ITaskRepository, TaskRepository>();
        serviceCollection.AddScoped<ICustomerRepository, CustomerRepository>();
        serviceCollection.AddScoped<IOperationRepository, OperationRepository>();
        serviceCollection.AddScoped<ICommentRepository, CommentRepository>();
        serviceCollection.AddScoped<IPhotoRepository, PhotoRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IBlobStorageComponent, BlobStorageComponent>();
    }

    private static void AddComponents(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        ConfigurePowerAutomateComponent(serviceCollection, configuration);
        ConfigureBlobStorageComponent(serviceCollection, configuration);
    }

    private static void ConfigurePowerAutomateComponent(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IPowerAutomateComponent, PowerAutomateComponent>();
        ConfigurePowerAutomateHttpClient(serviceCollection, configuration);
    }

    private static void ConfigurePowerAutomateHttpClient(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddHttpClient(Constants.HttpClientNames.PowerAutomate,
            c =>
            {
                var powerAutomateUriString = "https://prod-06.northeurope.logic.azure.com/workflows/57f5da58c3104842a94ee0306ed0e5a8/triggers/manual/paths/invoke";
                c.BaseAddress = new Uri(powerAutomateUriString);
                c.DefaultRequestHeaders.Add(
                    "X-Api-key",
                    configuration["PowerAutomateFlowApiKey"]);
            });
    }

    private static void ConfigureBlobStorageComponent(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IBlobStorageComponent, BlobStorageComponent>();
        serviceCollection.AddAzureClients(clientBuilder =>
        {
            //var connectionString = configuration[Constants.ConfigurationKeys.BlobStorageConnectionString];
            clientBuilder.AddBlobServiceClient(configuration["AzureStorageAccountAccessKey"]);
        });
    }
}
