using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Model.Units;
using Shard.RayanCedric.API.Model.Units.Managers;
using Shard.RayanCedric.API.Model.Users;

namespace Shard.RayanCedric.API.Model.Buildings.ConstructionBuildings;

[BsonDiscriminator("StarPort")]
public class StarPort : Building
{
    private const BuildingType STARPORT_TYPE = BuildingType.Starport;

    public StarPort() : base(STARPORT_TYPE)
    {
    }

    public Unit BuildUnit(User user, UnitType unitType, UnitCreationManager unitCreationManager)
    {
        if (!IsBuilt)
            throw new InvalidOperationException($"The StarPort with id {Id} is not built.");

        return unitCreationManager.BuildUnit(this, user, unitType);
    }
}