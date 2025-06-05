using Microsoft.AspNetCore.Mvc;
using Shard.RayanCedric.API.Contracts;
using Shard.RayanCedric.API.Gateway;
using Shard.RayanCedric.API.Model.Units;
using Shard.RayanCedric.API.Services;

namespace Shard.RayanCedric.API.Controllers;

/// <summary>
/// Controller for managing units.
/// </summary>
[Route("users/{userId}/[controller]")]
[ApiController]
public class UnitsController : ControllerBase
{
    private readonly UnitService _unitService;
    private readonly IShardGateway _shardGateway;
    private readonly ILogger<UnitsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitsController"/> class.
    /// </summary>
    /// <param name="unitService">The unit repository.</param>
    public UnitsController(UnitService unitService, IShardGateway shardGateway, ILogger<UnitsController> logger)
    {
        _unitService = unitService;
        _shardGateway = shardGateway;
        _logger = logger;
    }

    /// <summary>
    /// Returns all units of a user.
    /// </summary>
    /// <param name="userId">Id of the user whose units are requested.</param>
    /// <returns>A list of units.</returns>
    /// <response code="400">If the user id is null or empty.</response>
    /// <response code="404">If there is no user with such id.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<UnitContract>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<List<UnitContract>> GetUnits(string userId)
    {
        try
        {
            _logger.LogInformation($"GET units for {userId}");
            var units = _unitService.GetUnits(userId)
                .Select(unit => _unitService.GetUnitContract(unit))
                .ToList();

            return units;
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET units for {userId} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET units for {userId} failed: {e.Message}");
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Returns information about one single unit of a user.
    /// </summary>
    /// <param name="userId">Id of the user whose units are requested.</param>
    /// <param name="unitId">Id of the unit whose information are requested.</param>
    /// <returns>The unit.</returns>
    /// <response code="400">If one of the parameter is null or empty</response>
    /// <response code="404">If there is no user with such id, or if there is no unit with such id for that user.</response>
    [HttpGet("{unitId}")]
    [ProducesResponseType(typeof(UnitContract), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UnitContract>> GetUnit(string userId, string unitId)
    {
        try
        {
            _logger.LogInformation($"GET unit {unitId} for user {userId}");
            var unit = await _unitService.GetUnit(userId, unitId);

            return _unitService.GetUnitContract(unit);
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET unit {unitId} for user {userId} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET unit {unitId} for user {userId} failed: {e.Message}");
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Change the unit's status of a user. Right now, only its position (system and planet) can be changed.
    /// </summary>
    /// <param name="userId">Id of the user whose units are requested.</param>
    /// <param name="unitId">Id of the unit whose information are requested.</param>
    /// <param name="unit">The new unit details.</param>
    /// <returns>The updated unit.</returns>
    /// <response code="400">If there is no body, or if the id of the unit in the body is different than the one in the url.</response>
    /// <response code="404">If there is no user with such id, or if there is no unit with such id for that user.</response>
    [HttpPut("{unitId}")]
    [ProducesResponseType(typeof(UnitContract), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UnitContract>> UpdateUnit(string userId, string unitId, [FromBody] UnitContract unit)
    {
        try
        {
            _logger.LogInformation($"PUT move unit {unitId} for user {userId}");
            var client = HttpContext.User;
            Unit result;
            if (client.IsInRole("administrator"))
                result = _unitService.AddUnitAsAdmin(userId, unitId, unit);
            else if (client.IsInRole("shard"))
            {
                var serverName = client.Identity?.Name?[6..];
                result = _unitService.ReceiveUnitFromJump(userId, unitId, unit, serverName);
            }
            else 
                result = _unitService.UpdateUnit(userId, unitId, unit);

            if (unit.DestinationShard is null) 
                return _unitService.GetUnitContract(result);

            var owner = _unitService.CheckReadyToJump(userId, unit);
            
            var redirectUrl = await _shardGateway.ExecuteJump(unit.DestinationShard, owner, result);
            Response.Headers.Location = redirectUrl;
            return StatusCode(308);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError($"PUT unit {unitId} for user {userId} failed: {e.Message}");
            return Unauthorized(e.Message);
        }
        catch (Exception e) when (e is ArgumentNullException or InvalidOperationException)
        {
            _logger.LogError($"PUT unit {unitId} for user {userId} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"PUT move unit {unitId} for user {userId} failed: {e.Message}");
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Returns more detailed information about the unit's location of a user.
    /// </summary>
    /// <param name="userId">Id of the user whose units are requested.</param>
    /// <param name="unitId">Id of the unit whose information are requested.</param>
    /// <returns>The unit location.</returns>
    /// <response code="400">If one of the parameter is null or empty</response>
    /// <response code="404">If there is no user with such id, or if there is no unit with such id for that user.</response>
    [HttpGet("{unitId}/location")]
    [ProducesResponseType(typeof(UnitLocation), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UnitLocation>> GetLocation(string userId, string unitId)
    {
        try
        {
            _logger.LogInformation($"GET unit {unitId}'s location details for user {userId}");
            var location = await _unitService.GetLocation(userId, unitId);
            return location;
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET unit {unitId}'s location details for user {userId} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET unit {unitId}'s location details for user {userId} failed: {e.Message}");
            return NotFound(e.Message);
        }
    }
}