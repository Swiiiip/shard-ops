using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Model.Sector;

namespace Shard.RayanCedric.API.Model.Units.BasicUnits;

[BsonDiscriminator("Builder")]
public class Scout : Unit
{
    private const UnitType SCOUT_TYPE = UnitType.Scout;
    private const int SCOUT_HEALTH = 20;
    
    public Scout(StarSystem starSystem, Planet? planet) : base(SCOUT_TYPE, starSystem, planet, SCOUT_HEALTH)
    {
    }
}