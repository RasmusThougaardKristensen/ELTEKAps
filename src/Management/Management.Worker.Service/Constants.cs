namespace ELTEKAps.Management.Worker.Service;

public static class Constants
{
    public static class Service
    {
        public static string FullyQualifiedName => $"{Infrastructure.Constants.Service.BoundedContext}.{Infrastructure.Constants.Service.ServiceName}.Worker.Service";
        public static string ApplicationType => "Worker.Service";
    }

    public static class ServiceBus
    {
        public static string InputQueue => $"{Infrastructure.Constants.Service.BoundedContext}_{Infrastructure.Constants.Service.ServiceName}_input".ToLower();
    }

    public static class Rebus
    {
        public const string OperationId = "current-operation-id";
    }
}
