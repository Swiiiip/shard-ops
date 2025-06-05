using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Weapons;

namespace Shard.RayanCedric.API.Model.Units.CombatUnits;

[BsonDiscriminator("Bomber")]
public class Bomber : CombatUnit
{
    private const UnitType BOMBER_TYPE = UnitType.Bomber;
    private const int BOMBER_HEALTH = 50;
    private static readonly List<Weapon> BOMBER_WEAPONS = [new Bomb()];
    private const int DAMAGE_REDUCTION_FACTOR_FROM_CRUISERS = 10;

    public Bomber(StarSystem starSystem, Planet? planet)
        : base(BOMBER_TYPE, BOMBER_WEAPONS, starSystem, planet, BOMBER_HEALTH)
    {
    }
    
    public override void TakeDamage(int damage, Unit attacker)
    {
        if (attacker is Cruiser)
            damage /= DAMAGE_REDUCTION_FACTOR_FROM_CRUISERS;
        
        base.TakeDamage(damage, attacker);
    }
}