using Microsoft.AspNetCore.Mvc;
using Shard.API.Contracts;
using Shard.API.Model.Users;
using UserService = Shard.API.Services.UserService;

namespace Shard.API.Controllers;

[Route("[controller]/{userId}")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPut]
    public ActionResult<UserContract> CreateUser(string userId, [FromBody] UserContract user)
    {
        try
        {
            _logger.LogInformation($"PUT Creating user with id {userId}");
            var client = HttpContext.User;
            User result;
            if (client.IsInRole("administrator"))
                result = _userService.UpdateUser(userId, user);
            else if (client.IsInRole("shard"))
                result = _userService.CopyUser(user);
            else 
                result = _userService.CreateUser(userId, user);
            
            return _userService.GetUserContract(result);
        } 
        catch (Exception e) when (e is ArgumentNullException or InvalidOperationException)
        {
            _logger.LogError($"PUT Creating user with id {userId} failed: {e.Message}");
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public ActionResult<UserContract> GetUser(string userId)
    {
        try
        {
            _logger.LogError($"GET user with id {userId}");
            var user = _userService.GetUser(userId);
            return _userService.GetUserContract(user);
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET user with id {userId} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e) 
        { 
            _logger.LogError($"GET user with id {userId} failed: {e.Message}");
            return NotFound(e.Message); 
        }
    }
}