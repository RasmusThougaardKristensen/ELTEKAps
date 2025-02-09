namespace ELTEKAps.Management.ApplicationServices.PowerAutomate;

public interface IPowerAutomateService
{
    Task TaskUpdated(Guid taskId);
    Task TaskUpdateFailed(Guid taskId, string errorMessage);
}