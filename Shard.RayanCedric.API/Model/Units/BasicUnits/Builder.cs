using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Model.Buildings;
using Shard.RayanCedric.API.Model.Sector;

namespace Shard.RayanCedric.API.Model.Units.BasicUnits;

[BsonDiscriminator("Builder")]
public class Builder : Unit
{
    private const UnitType BUILDER_TYPE = UnitType.Builder;
    private const int BUILDER_HEALTH = 20;

    public readonly List<Building> Buildings;
    
    public Builder(StarSystem starSystem, Planet? planet) : base(BUILDER_TYPE, starSystem, planet, BUILDER_HEALTH)
    {
        Buildings = [];
    }
    
    public void AddBuilding(Building building)
    {
        Buildings.Add(building);
    }

    public void RemoveBuilding(Building building)
    {
        Buildings.Remove(building);
    }
}