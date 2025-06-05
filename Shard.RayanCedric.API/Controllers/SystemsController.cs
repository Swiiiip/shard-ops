using Microsoft.AspNetCore.Mvc;
using Shard.RayanCedric.API.Contracts;
using Shard.RayanCedric.API.Services;

namespace Shard.RayanCedric.API.Controllers;

/// <summary>
/// Controller for managing star systems and their planets.
/// </summary>
[Route("[controller]")]
[ApiController]
public class SystemsController : ControllerBase
{
    private readonly SectorService _sectorService;
    private readonly ILogger<SystemsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemsController"/> class.
    /// </summary>
    /// <param name="sectorService">The sector repository.</param>
    public SystemsController(SectorService sectorService, ILogger<SystemsController> logger)
    {
        _sectorService = sectorService;
        _logger = logger;
    }

    /// <summary>
    /// Fetch all systems, and their planets
    /// </summary>
    /// <returns>A list of star systems.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<StarSystemContract>), 200)]
    public ActionResult<List<StarSystemContract>> GetSystems()
    {
        try
        {
            _logger.LogInformation("GET systems");
            var starSystems = _sectorService.GetSystems();

            return starSystems.Select(system => _sectorService.GetStarSystemContract(system))
                .ToList();
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET systems failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET systems failed: {e.Message}");
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Fetch a single system, and all its planets.
    /// </summary>
    /// <param name="systemName">Name (identifier) of the system</param>
    /// <returns>The star system.</returns>
    /// <response code="404">If the system does not exist</response>
    [HttpGet("{systemName}")]
    [ProducesResponseType(typeof(StarSystemContract), 200)]
    [ProducesResponseType(404)]
    public ActionResult<StarSystemContract> GetSystem(string systemName)
    {
        try
        {
            _logger.LogInformation($"GET system {systemName}");
            var system = _sectorService.GetSystem(systemName);
            return _sectorService.GetStarSystemContract(system);
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET system {systemName} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET system {systemName} failed: {e.Message}");
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Fetch all planets of a single system.
    /// </summary>
    /// <param name="systemName">Name (identifier) of the system</param>
    /// <returns>A list of planets.</returns>
    /// <response code="404">If the system does not exist</response>
    [HttpGet("{systemName}/planets")]
    [ProducesResponseType(typeof(List<PlanetContract>), 200)]
    [ProducesResponseType(404)]
    public ActionResult<List<PlanetContract>> GetPlanets(string systemName)
    {
        try
        {
            _logger.LogInformation($"GET planets from system {systemName}");
            var planets = _sectorService.GetPlanets(systemName);
            return planets.Select(planet => _sectorService.GetPlanetContract(planet))
                .ToList();
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET planets from system {systemName} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET planets from system {systemName} failed: {e.Message}");
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Fetch a single planet of a system.
    /// </summary>
    /// <param name="systemName">Name (identifier) of the system</param>
    /// <param name="planetName">Name (identifier) of the planet</param>
    /// <returns>The planet.</returns>
    /// <response code="404">If the system does not exist, or if the planet does not exist in that system</response>
    [HttpGet("{systemName}/planets/{planetName}")]
    [ProducesResponseType(typeof(PlanetContract), 200)]
    [ProducesResponseType(404)]
    public ActionResult<PlanetContract> GetPlanet(string systemName, string planetName)
    {
        try
        {
            _logger.LogInformation($"GET planet {planetName} from system {systemName}");
            var planet = _sectorService.GetPlanet(systemName, planetName);
            return _sectorService.GetPlanetContract(planet);
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError($"GET planet {planetName} from system {systemName} failed: {e.Message}");
            return BadRequest(e.Message);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError($"GET planet {planetName} from system {systemName} failed: {e.Message}");
            return NotFound(e.Message);
        }
    }
}
