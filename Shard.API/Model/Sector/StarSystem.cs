using Shard.Shared.Core;

namespace Shard.API.Model.Sector;

public class StarSystem
{
    public IReadOnlyList<Planet> Planets { get; }

    public string Name { get; }
    
    public StarSystem(SystemSpecification systemSpecification)
    {
        Planets = systemSpecification.Planets
            .Select(specification => new Planet(specification))
            .ToList();

        Name = systemSpecification.Name;
    }
    
    public Planet this[string? planetName]
    {
        get
        {
            if (string.IsNullOrEmpty(planetName))
                throw new ArgumentNullException(nameof(planetName), "Planet name cannot be null or empty");

            var planet = Planets.FirstOrDefault(planet => planet.Name == planetName);
            
            return planet ?? throw new KeyNotFoundException($"Planet with name '{planetName}' not found in this star system.");
        }
    }
}