using Ardalis.ApiEndpoints;
using ELTEKAps.Management.ApplicationServices.Customers.Get;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Customer.GetCustomer;

public class GetCustomerEndpoint : EndpointBaseAsync.WithRequest<Guid>.WithActionResult<CustomerResponse>
{
    private readonly IGetCustomerByIdService _getCustomerByIdService;

    public GetCustomerEndpoint(IGetCustomerByIdService getCustomerByIdService)
    {
        _getCustomerByIdService = getCustomerByIdService;
    }

    [HttpGet("api/customers/{customerId:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get customer by id",
        Description = "Get customer by id",
        OperationId = "GetCustomer",
        Tags = new[] { Constants.ApiTags.Customer })
    ]
    public override async Task<ActionResult<CustomerResponse>> HandleAsync([FromRoute] Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _getCustomerByIdService.GetCustomerById(customerId);

        if (customer == null)
        {
            return Problem(title: "Customer could not be found",
                detail: $"Customer having id: '{customerId}' not found", statusCode: StatusCodes.Status404NotFound);
        }

        var customerResponse = CustomerMapper.ToResponseModel(customer);

        return new ActionResult<CustomerResponse>(customerResponse);
    }
}
