namespace ELTEKAps.Management.Infrastructure;
public static class Constants
{
    public static class Service
    {
        public const string ServiceName = "Management";
        public const string BoundedContext = "ELTEKAps";
    }

    public static class Environment
    {
        public static bool IsDevelopment
        {
            get
            {
                var variable =
                    System.Environment.GetEnvironmentVariable(ConfigurationKeys.AspnetCoreDevelopmentEnvironment);
                return string.IsNullOrEmpty(variable) == false && variable.Equals("Development");
            }
        }

        public static bool IsGeneratingApi
        {
            get
            {
                var variable =
                    System.Environment.GetEnvironmentVariable(ConfigurationKeys.AspnetCoreDevelopmentEnvironment);
                return string.IsNullOrEmpty(variable) == false && variable.Equals("SwaggerGen");
            }
        }
    }

    public static class ServiceBus
    {
        public static string InputQueue => $"{Service.BoundedContext}_{Service.ServiceName}_input".ToLower();
    }


    public static class ConfigurationKeys
    {
        public const string AspnetCoreDevelopmentEnvironment = "ASPNETCORE_ENVIRONMENT";
        public const string ServiceBusConnectionString = "ServiceBusConnectionString";
        public const string SqlDbConnectionString = "SqlDbConnectionString";
        public const string BlobStorageConnectionString = "BlobStorageConnectionString";
    }

    public static class HttpClientNames
    {
        public static string PowerAutomate => "PowerAutomate";
    }

    public static class FilePaths
    {
        public static string BlobStorage => "webapi";
    }
}
