using ELTEKAps.Management.ApplicationServices.BlobService.CreateBlobBlock;
using ELTEKAps.Management.ApplicationServices.Comments.Create;
using ELTEKAps.Management.ApplicationServices.Comments.SoftDelete;
using ELTEKAps.Management.ApplicationServices.Comments.Update;
using ELTEKAps.Management.ApplicationServices.Customers.Create;
using ELTEKAps.Management.ApplicationServices.Customers.Get;
using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Photos.Create;
using ELTEKAps.Management.ApplicationServices.Photos.SoftDelete;
using ELTEKAps.Management.ApplicationServices.PowerAutomate;
using ELTEKAps.Management.ApplicationServices.SignalR;
using ELTEKAps.Management.ApplicationServices.Tasks.Create;
using ELTEKAps.Management.ApplicationServices.Tasks.Get;
using ELTEKAps.Management.ApplicationServices.Tasks.SoftDelete;
using ELTEKAps.Management.ApplicationServices.Tasks.Update;
using ELTEKAps.Management.ApplicationServices.Users.Create;
using ELTEKAps.Management.ApplicationServices.Users.Get;
using Management.Worker.Service.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace ELTEKAps.Management.ApplicationServices.Extensions;
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServiceServices(this IServiceCollection services)
    {
        //Operations
        services.AddScoped<IOperationService, OperationService>();

        //Tasks
        services.AddScoped<IGetTaskByIdService, GetTaskByIdService>();
        services.AddScoped<IGetTasksService, GetTasksService>();
        services.AddScoped<ICreateTaskService, CreateTaskService>();
        services.AddScoped<IUpdateTaskService, UpdateTaskService>();
        services.AddScoped<ISoftDeleteTaskService, SoftDeleteTaskService>();


        //Comments
        services.AddScoped<ICreateCommentService, CreateCommentService>();
        services.AddScoped<IUpdateCommentService, UpdateCommentService>();
        services.AddScoped<ISoftDeleteCommentService, SoftDeleteCommentService>();

        //Photos
        services.AddScoped<ICreatePhotoService, CreatePhotoService>();
        services.AddScoped<ISoftDeletePhotoService, SoftDeletePhotoService>();

        //Users        
        services.AddScoped<ICreateUserService, CreateUserService>();
        services.AddScoped<IGetUserByFirebaseIdService, GetUserByFirebaseIdService>();
        services.AddScoped<IGetUsersService, GetUsersService>();

        //Customer
        services.AddScoped<ICreateCustomerService, CreateCustomerService>();
        services.AddScoped<IGetCustomerByIdService, GetCustomerByIdService>();
        services.AddScoped<IGetCustomersService, GetCustomersService>();

        //Blob Storage service
        services.AddScoped<ICreateBlobBlockService, CreateBlobBlockService>();

        //Power Automate
        services.AddScoped<IPowerAutomateService, PowerAutomateService>();

        //IMessage service
        services.AddTransient<IMessageService, MessageService>();

        return services;
    }
}
