using Shard.API.Model.Sector;
using Shard.API.Model.Units.BasicUnits;

namespace Shard.API.Model.Units.Factories;
public class ScoutFactory : IUnitFactory
{
    public Unit CreateUnit(StarSystem starSystem, Planet? planet)
    {
        return new Scout(starSystem, planet);
    }
}