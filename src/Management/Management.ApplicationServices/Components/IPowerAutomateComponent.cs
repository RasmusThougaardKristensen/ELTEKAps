namespace ELTEKAps.Management.ApplicationServices.Components;
public interface IPowerAutomateComponent
{
    Task Notify(string recipientEmail, string username, string taskTitle);
}
