using ELTEKAps.Management.Api.Service.Authentication;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.BlobService.CreateBlobBlock;
using ELTEKAps.Management.ApplicationServices.Extensions;
using ELTEKAps.Management.Infrastructure.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ELTEKAps.Management.Api.Service;

public class Startup
{
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //Figure out if AddMvc is used
        services.AddMvc()
            .AddJsonOptions(options =>
            {
                var enumConvertor = new JsonStringEnumConverter();
                options.JsonSerializerOptions.Converters.Add(enumConvertor);
            });
        services.AddApplicationServiceServices();
        services.AddHttpContextAccessor();
        services.AddScoped<TaskEnvironment>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ClaimsPrincipal>(p => p.GetRequiredService<IHttpContextAccessor>().HttpContext?.User);
        services.AddSingleton(FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(Configuration["FirebaseCredentials"]),
        }));
        services.AddHttpClient<CreateBlobBlockService>();
        if (Configuration["UseAuthentication"] == "True")
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, FirebaseUserAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme,
                    (o) =>
                    {

                    });
        }
        else
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, LocalTestAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme,
                    (o) =>
                    {

                    });
        }

        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SupportNonNullableReferenceTypes();
            c.EnableAnnotations();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsStaging())
            app.UseDeveloperExceptionPage();
        app.UseCors(x =>
        {

            x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
        app.UseSwagger();

        if (env.IsDevelopment() || env.IsStaging() || Infrastructure.Constants.Environment.IsGeneratingApi)
            app.UseDeveloperExceptionPage();

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = string.Empty;
            options.SwaggerEndpoint(Constants.Routes.SwaggerEndpoint, Constants.Services.ApiName);
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSwagger();
        });

        if (env.IsEnvironment("integration-test") || Infrastructure.Constants.Environment.IsGeneratingApi)
            return;

        app.EnsureDatabaseMigrated();
        //If you want to add rebus you need to add it here!
    }
}
