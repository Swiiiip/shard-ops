using Microsoft.Extensions.Options;
using Shard.RayanCedric.API.Model.Sector;
using Shard.Shared.Core;

namespace Shard.RayanCedric.API.Repositories.Universe;

public class SectorRepository : ISectorRepository
{
    
    private readonly Sector _sector;    
    private const int NO_SYSTEM_FOUND = 0;

    public SectorRepository(IOptions<MapGeneratorOptions> options)
    {
        var sectorSpec = new MapGenerator(options.Value).Generate();
        _sector = new Sector(sectorSpec);
        if (_sector.Systems.Count == NO_SYSTEM_FOUND)
            throw new InvalidOperationException("No star systems available.");
    }
    
    public List<StarSystem> FindAllStarSystems()
    {
        return _sector.Systems.ToList();
    }

    public StarSystem FindStarSystemByName(string systemName)
    {
        return _sector[systemName];
    }

    public List<Planet> FindAllPlanetsByStarSystemName(string systemName)
    {
        return _sector[systemName].Planets.ToList();
    }

    public Planet FindPlanetByStarSystemNameAndPlanetName(string systemName, string? planetName)
    {
        return _sector[systemName][planetName];
    }

    public StarSystem? FindStarSystemByPlanet(Planet? planet)
    {
        return FindAllStarSystems()
            .FirstOrDefault(system => system.Planets.Contains(planet));
    }

    public Planet? FindPlanetByPlanetName(string? planetName)
    {
        if (string.IsNullOrEmpty(planetName))
            return null;
        
        var allStarSystems = FindAllStarSystems();
        
        foreach (var planetToFind in allStarSystems
                     .Select(starSystem => starSystem.Planets.FirstOrDefault(planet => planet.Name == planetName))
                     .OfType<Planet>())
            return planetToFind;

        throw new KeyNotFoundException($"Planet with name '{planetName}' not found in any star system.");
    }

    public StarSystem FindRandomStarSystem()
    {
        var allStarSystems = FindAllStarSystems();
        var randomIndex = new Random().Next(allStarSystems.Count);
        return allStarSystems[randomIndex];
    }
}