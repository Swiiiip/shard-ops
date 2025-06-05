using Microsoft.AspNetCore.Mvc;
using Shard.API.Contracts;
using Shard.API.Model.Users;
using UserService = Shard.API.Services.UserService;

namespace Shard.API.Controllers;

/// <summary>
/// Controller for managing users.
/// </summary>
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

    /// <summary>
    /// Creates or updates a user.
    /// </summary>
    /// <param name="userId">Id of the user to create or update.</param>
    /// <param name="user">User details.</param>
    /// <returns>The created or updated user.</returns>
    /// <response code="200">Returns the user after creation or update.</response>
    /// <response code="400">If the request is invalid.</response>
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

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="userId">Id of the user to retrieve.</param>
    /// <returns>The user.</returns>
    /// <response code="200">Returns the user.</response>
    /// <response code="400">If the userId is invalid.</response>
    /// <response code="404">If the user is not found.</response>
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