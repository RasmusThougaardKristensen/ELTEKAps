using Ardalis.ApiEndpoints;
using ELTEKAps.Management.ApplicationServices.Users.Get;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Users;

[Authorize]
public class GetUsersEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult<IEnumerable<UserResponse>>
{
    private readonly IGetUsersService _getUsersService;

    public GetUsersEndpoint(IGetUsersService getUsersService)
    {
        _getUsersService = getUsersService;
    }

    [HttpGet("api/users")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Retrieve a list of all users",
        OperationId = "GetUsers",
        Tags = new[] { Constants.ApiTags.User })
    ]
    public override async Task<ActionResult<IEnumerable<UserResponse>>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var users = await _getUsersService.GetUsers();

        var userResponses = UserResponseMapper.ToResponseModels(users).ToList();

        return new ActionResult<IEnumerable<UserResponse>>(userResponses);
    }
}
