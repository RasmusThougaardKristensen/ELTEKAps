using ELTEKAps.Management.Infrastructure.Installers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace ELTEKAps.Management.Api.Service.Installers;

public class SwaggerInstaller : IDependencyInstaller
{
    public void Install(IServiceCollection serviceCollection, DependencyInstallerOptions options)
    {
        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Key Auth", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "ApiKey must appear in header",
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = JwtBearerDefaults.AuthenticationScheme,

            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new string[] {}
                }
            });
        });
    }
}
