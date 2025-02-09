using ELTEKAps.Management.ApplicationServices.Extensions;
using Management.Worker.Service.SignalR;
using Rebus.Config;

namespace ELTEKAps.Management.Worker.Service;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServiceServices();
        services.AddSignalR();
        services.AddCors();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    builder.WithOrigins("https://mango-coast-0dc0b2603.4.azurestaticapps.net")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.ApplicationServices.StartRebus();
        app.UseCors("AllowSpecificOrigin");

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<WorkerHub>("/workerHub");
        });
    }
}
