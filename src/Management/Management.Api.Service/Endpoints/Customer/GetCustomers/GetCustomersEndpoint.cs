using Ardalis.ApiEndpoints;
using ELTEKAps.Management.ApplicationServices.Customers.Get;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Customer.GetCustomers
{
    public class GetCustomersEndpoint : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<IEnumerable<CustomerResponse>>
    {
        private readonly IGetCustomersService _getCustomersService;

        public GetCustomersEndpoint(IGetCustomersService getCustomersService)
        {
            _getCustomersService = getCustomersService;
        }

        [HttpGet("api/customers")]
        [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get all customers",
            Description = "Retrieve a list of all customers",
            OperationId = "GetCustomers",
            Tags = new[] { Constants.ApiTags.Customer })
        ]
        public override async Task<ActionResult<IEnumerable<CustomerResponse>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var customers = await _getCustomersService.GetCustomers();

                if (!customers.Any())
                {
                    return new ActionResult<IEnumerable<CustomerResponse>>(Array.Empty<CustomerResponse>());
                }

                var customerResponses = customers.Select(CustomerMapper.ToResponseModel);

                return new ActionResult<IEnumerable<CustomerResponse>>(customerResponses);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "An error occurred while fetching customers",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
