using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Weapons;

namespace Shard.RayanCedric.API.Model.Units.CombatUnits;

[BsonDiscriminator("Fighter")]
public class Fighter : CombatUnit
{
    private const UnitType FIGHTER_TYPE = UnitType.Fighter;
    private const int FIGHTER_HEALTH = 80;
    private static readonly List<Weapon> FIGHTER_WEAPONS = [new Cannon()];

    public Fighter(StarSystem starSystem, Planet? planet) : base(FIGHTER_TYPE, FIGHTER_WEAPONS, starSystem, planet, FIGHTER_HEALTH)
    {
    }
}