using Shard.API.Model.Sector;
using Shard.API.Model.Units.CombatUnits;

namespace Shard.API.Model.Units.Factories;

public class CruiserFactory : IUnitFactory
{
    public Unit CreateUnit(StarSystem starSystem, Planet? planet)
    {
        return new Cruiser(starSystem, planet);
    }
}