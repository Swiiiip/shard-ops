using Shard.API.Model.Sector;

namespace Shard.API.Model.Units.TransportUnits;

public class Cargo : Unit
{
    private const UnitType CARGO_TYPE = UnitType.Cargo;
    private const int CARGO_HEALTH = 100;
    
    public Cargo(StarSystem starSystem, Planet? planet) : base(CARGO_TYPE, starSystem, planet, CARGO_HEALTH)
    {
    }
}