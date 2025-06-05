using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Units.TransportUnits;

namespace Shard.RayanCedric.API.Model.Units.Factories;

public class CargoFactory : IUnitFactory
{
    public Unit CreateUnit(StarSystem starSystem, Planet? planet)
    {
        return new Cargo(starSystem, planet);
    }
}