using Shard.API.Model.Buildings.EconomicBuildings;
using Shard.API.Model.Sector;

namespace Shard.API.Model.Buildings.Factories;

public class MineFactory : IBuildingFactory
{
    public Building CreateBuilding(ResourceCategory? resourceKind = null)
    {
        return new Mine(resourceKind);
    }
}