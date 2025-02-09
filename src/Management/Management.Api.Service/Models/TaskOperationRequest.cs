using Microsoft.AspNetCore.Mvc;

namespace ELTEKAps.Management.Api.Service.Models;

public class TaskOperationRequest<T> : OperationRequest
{
    [FromBody] public T Details { get; set; }
}
