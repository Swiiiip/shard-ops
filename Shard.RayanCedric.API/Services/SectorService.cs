using Shard.RayanCedric.API.Contracts;
using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Units;
using Shard.RayanCedric.API.Repositories.Universe;

namespace Shard.RayanCedric.API.Services;

public class SectorService
{
    private readonly ISectorRepository _sectorRepository;

    public SectorService(ISectorRepository sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }
    
    public List<StarSystem> GetSystems()
    {
        return _sectorRepository.FindAllStarSystems();
    }
  
    public StarSystem GetSystem(string systemName)
    {
        return _sectorRepository.FindStarSystemByName(systemName);
    }

    public List<Planet> GetPlanets(string systemName)
    {
        return _sectorRepository.FindAllPlanetsByStarSystemName(systemName);
    }

    public Planet GetPlanet(string systemName, string? planetName)
    {
        return _sectorRepository.FindPlanetByStarSystemNameAndPlanetName(systemName, planetName);
    }

    public StarSystem? GetSystemOfPlanet(Planet? planet)
    {
        return _sectorRepository.FindStarSystemByPlanet(planet);
    }

    public Planet? GetPlanetFromName(string? planetName)
    {
        return _sectorRepository.FindPlanetByPlanetName(planetName);
    }
    
    public StarSystem GetRandomStarSystem()
    {
        return _sectorRepository.FindRandomStarSystem();
    }
    
    public List<Unit> GetCloseEnemies(Unit unit, UserService userService)
    {
        var owner = userService.GetUserByUnitId(unit.Id);

        var enemiesUnits = userService.GetAllUsers()
            .Where(user => user.Id != owner?.Id) 
            .SelectMany(user => user.Units)
            .ToList();

        return unit.Planet != null
            ? enemiesUnits.Where(u => u.Planet == unit.Planet).ToList()
            : enemiesUnits.Where(u => u.StarSystem == unit.StarSystem).ToList();
    }

    public PlanetContract GetPlanetContract(Planet planet)
    {
        return new PlanetContract(planet.Name, planet.Size);
    }

    public StarSystemContract GetStarSystemContract(StarSystem system)
    {
        return new StarSystemContract(system.Name, system.Planets
            .Select(GetPlanetContract)
            .ToList());
    }
}