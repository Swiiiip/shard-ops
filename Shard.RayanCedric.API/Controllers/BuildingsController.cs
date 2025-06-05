using Microsoft.AspNetCore.Mvc;
using Shard.RayanCedric.API.Contracts;
using Shard.RayanCedric.API.Services;

namespace Shard.RayanCedric.API.Controllers;

/// <summary>
/// Controller for managing buildings.
/// </summary>
[Route("users/{userId}/[controller]")]
[ApiController]
public class BuildingsController : ControllerBase
{
    private readonly BuildingService _buildingService;
    private readonly UnitService _unitService;
    private readonly ILogger<BuildingsController> _logger;

    public BuildingsController(BuildingService buildingService, UnitService unitService, ILogger<BuildingsController> logger)
    {
        _buildingService = buildingService;
        _unitService = unitService;
        _logger = logger;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(BuildingContract), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<BuildingContract>> CreateBuilding(string userId, [FromBody] BuildingContract building)
    {
        try
        {
            _logger.LogInformation($"POST Creating building {building.Type} for user with id {userId}");
            var newBuilding = await _buildingService.CreateBuilding(userId, building);
            
            return _buildingService.GetBuildingContract(newBuilding);

        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"POST creating building failed: {e.Message}");
            return NotFound(e.Message);
        } 
        catch (Exception e) when (e is ArgumentNullException or InvalidOperationException)
        {
            _logger.LogError($"POST creating building failed: {e.Message}");
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BuildingContract>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<List<BuildingContract>> GetBuildings(string userId)
    {
        try
        {
            _logger.LogInformation($"GET getting buildings for user {userId}");
            var buildings = _buildingService.GetBuildings(userId)
                .Select(building => _buildingService.GetBuildingContract(building))
                .ToList();
        
            return buildings;
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET getting buildings failed: {e.Message}");
            return NotFound(e.Message);
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET getting buildings failed: {e.Message}");
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{buildingId}")]
    [ProducesResponseType(typeof(BuildingContract), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<BuildingContract>> GetBuilding(string userId, string buildingId)
    {
        try
        {
            _logger.LogInformation($"GET building {buildingId} for user {userId}");
            var building = await _buildingService.GetBuilding(userId, buildingId);
            
            return _buildingService.GetBuildingContract(building);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET building failed: {e.Message}");
            return NotFound(e.Message);
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET building failed: {e.Message}");
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{starPortId}/queue")]
    [ProducesResponseType(typeof(UnitContract), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UnitContract>> BuildUnit(string userId, string starPortId, [FromBody] UnitContract unit)
    {
        try
        {
            _logger.LogInformation($"POST building unit for user {userId} with starPort {starPortId}");
            var builtUnit = await _buildingService.BuildUnit(userId, starPortId, unit);
            return _unitService.GetUnitContract(builtUnit);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"POST building unit for user {userId} with starPort {starPortId} failed: {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e) when (e is ArgumentNullException or InvalidOperationException)
        {
            _logger.LogError($"POST building unit for user {userId} with starPort {starPortId} failed: {e.Message}");
            return BadRequest(e.Message);
        }
    }
}