using Shard.API.Model.Sector;

namespace Shard.API.Repositories.Universe;

public interface ISectorRepository
{
    List<StarSystem> FindAllStarSystems();
    StarSystem FindStarSystemByName(string systemName);
    List<Planet> FindAllPlanetsByStarSystemName(string systemName);
    Planet FindPlanetByStarSystemNameAndPlanetName(string systemName, string? planetName);
    StarSystem? FindStarSystemByPlanet(Planet? planet);
    Planet? FindPlanetByPlanetName(string? planetName);
    StarSystem FindRandomStarSystem();
}