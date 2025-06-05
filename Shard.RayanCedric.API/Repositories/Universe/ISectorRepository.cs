using Shard.RayanCedric.API.Model.Sector;

namespace Shard.RayanCedric.API.Repositories.Universe;

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