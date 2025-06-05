using MongoDB.Bson.Serialization.Attributes;
using Shard.API.Model.Sector;
using Shard.API.Model.Weapons;

namespace Shard.API.Model.Units.CombatUnits;

[BsonDiscriminator("CombatUnit")]
[BsonKnownTypes(typeof(Bomber), typeof(Fighter), typeof(Cruiser))]
public abstract class CombatUnit : Unit
{
    public readonly List<Weapon> Weapons;
    
    protected CombatUnit(UnitType type, List<Weapon> weapons, StarSystem starSystem, Planet? planet, int health) : base(type, starSystem, planet, health)
    {
        Weapons = weapons;
    }
}