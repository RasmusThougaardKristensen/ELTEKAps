using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Endpoints.Operations.GetOperation;
using ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Customers.Create;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Customer.Create;

[Authorize]
public class CreateCustomerEndpoint : EndpointBaseAsync.WithRequest<CreateCustomerRequest>.WithoutResult
{
    private readonly ICreateCustomerService _createCustomerService;
    private readonly TaskEnvironment _env;

    public CreateCustomerEndpoint(ICreateCustomerService createCustomerService, TaskEnvironment env)
    {
        _createCustomerService = createCustomerService;
        _env = env;
    }

    [HttpPost("api/customers")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Create Customer",
        Description = "Create customer",
        OperationId = "Customer",
        Tags = new[] { Constants.ApiTags.Customer }
    )]
    public override async Task<ActionResult> HandleAsync([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var operationResult = await _createCustomerService.RequestCreateCustomer(
            request.Details.CustomerName,
            request.Details.PhoneNumber,
            request.Details.Email,
            new OperationDetails(Guid.NewGuid().ToString())
        );

        return operationResult.Status switch
        {
            OperationResultStatus.Accepted => new AcceptedResult(
                new Uri(GetOperationByRequestIdEndpoint.GetRelativePath(operationResult.GetOperation()), UriKind.Relative),
                new OperationAcceptedResponse(operationResult.GetOperation().RequestId)),

            OperationResultStatus.InvalidState => Problem(
                title: "Cannot create task",
                detail: operationResult.GetMessage(),
                statusCode: StatusCodes.Status400BadRequest
            ),

            _ => Problem(
                title: "Unknown error requesting to create task",
                detail: "Unknown error - check logs",
                statusCode: StatusCodes.Status500InternalServerError
            ),
        };
    }
}

public abstract class CustomerRequest<T>
{
    [FromBody] public T Details { get; set; }
}

public sealed class CreateCustomerRequest : CustomerRequest<CreateCustomerDetails> { }

[SwaggerSchema(Nullable = false)]
public sealed class CreateCustomerDetails
{
    public string CustomerName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}