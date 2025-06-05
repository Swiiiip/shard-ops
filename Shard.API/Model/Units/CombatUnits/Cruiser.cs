using MongoDB.Bson.Serialization.Attributes;
using Shard.API.Model.Sector;
using Shard.API.Model.Weapons;

namespace Shard.API.Model.Units.CombatUnits;

[BsonDiscriminator("Cruiser")]
public class Cruiser : CombatUnit
{
    private const UnitType CRUISER_TYPE = UnitType.Cruiser;
    private const int CRUISER_HEALTH = 400;
    private static readonly List<Weapon> CRUISER_WEAPONS = [new Cannon(), new Cannon(), new Cannon(), new Cannon()];
    
    public Cruiser(StarSystem starSystem, Planet? planet) 
        : base(CRUISER_TYPE, CRUISER_WEAPONS, starSystem, planet, CRUISER_HEALTH)
    {
    }
}