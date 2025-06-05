using Shard.RayanCedric.API.Model.Buildings.EconomicBuildings;
using Shard.RayanCedric.API.Model.Sector;

namespace Shard.RayanCedric.API.Model.Buildings.Factories;

public class MineFactory : IBuildingFactory
{
    public Building CreateBuilding(ResourceCategory? resourceKind = null)
    {
        return new Mine(resourceKind);
    }
}