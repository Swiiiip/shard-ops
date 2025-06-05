using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Units.BasicUnits;

namespace Shard.RayanCedric.API.Model.Units.Factories;
public class ScoutFactory : IUnitFactory
{
    public Unit CreateUnit(StarSystem starSystem, Planet? planet)
    {
        return new Scout(starSystem, planet);
    }
}