using Shard.Shared.Core;

namespace Shard.RayanCedric.API.Model.Sector;

public class Sector
{
    public IReadOnlyList<StarSystem> Systems { get; }

    public Sector(SectorSpecification sectorSpecficition)
     {
         Systems = sectorSpecficition.Systems
             .Select(specification => new StarSystem(specification))
             .ToList();
     }

    public StarSystem this[string systemName]
    {
        get
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName), "System name cannot be null or empty");

            var system = Systems.FirstOrDefault(system => system.Name == systemName);
            return system ?? throw new KeyNotFoundException($"System with name '{systemName}' not found in this sector.");
        }
    }
}