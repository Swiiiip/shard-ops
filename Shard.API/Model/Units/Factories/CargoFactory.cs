using Shard.API.Model.Sector;
using Shard.API.Model.Units.TransportUnits;

namespace Shard.API.Model.Units.Factories;

public class CargoFactory : IUnitFactory
{
    public Unit CreateUnit(StarSystem starSystem, Planet? planet)
    {
        return new Cargo(starSystem, planet);
    }
}