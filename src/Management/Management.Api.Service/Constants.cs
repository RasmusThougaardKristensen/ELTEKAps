namespace ELTEKAps.Management.Api.Service;

public static class Constants
{
    public static class Routes
    {
        public const string SwaggerEndpoint = "/swagger/v1/swagger.json";
    }

    public static class Services
    {
        public static string ApiName => $"{Infrastructure.Constants.Service.BoundedContext}.{Infrastructure.Constants.Service.ServiceName} API";
    }

    public static class ServiceBus
    {
        public static string InputQueue => $"{Infrastructure.Constants.Service.BoundedContext}_{Infrastructure.Constants.Service.ServiceName}_input".ToLowerInvariant();
    }

    public static class Service
    {
        public static string ApiName => $"{Infrastructure.Constants.Service.BoundedContext}.{Infrastructure.Constants.Service.ServiceName} API";
        public static string FullyQualifiedName => $"{Infrastructure.Constants.Service.BoundedContext}.{Infrastructure.Constants.Service.ServiceName}.Api.Service";
        public static string ApplicationType => "Api.Service";
    }

    public static class ApiTags
    {
        public const string Task = "Task";
        public const string User = "User";
        public const string PowerAutomate = "PowerAutomate";
        public const string Customer = "Customer";
        public const string Comment = "Comment";
    }
}
