using BlazorERP.Api.Mappings;
using BlazorERP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlazorERP.Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet(ApiEndpoints.Users.Get)]
    public async Task<IActionResult> Get([FromRoute] int userId, CancellationToken token)
    {
        var user = await _userService.GetAsync(userId, token);

        if (user is null)
        {
            return NotFound();
        }

        var response = user.MapToResponse();
        return Ok(response);
    }
}
