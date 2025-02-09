using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ELTEKAps.Management.Api.Service.Models;

public abstract class OperationRequest
{
    [FromHeader(Name = "created-by-user-id")][Required] public string CreatedByUserId { get; set; }
}
