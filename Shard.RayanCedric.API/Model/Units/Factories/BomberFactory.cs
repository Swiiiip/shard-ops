using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Units.CombatUnits;

namespace Shard.RayanCedric.API.Model.Units.Factories;

public class BomberFactory : IUnitFactory
{
    public Unit CreateUnit(StarSystem starSystem, Planet? planet)
    {
        return new Bomber(starSystem, planet);
    }
}