using Shard.API.Model.Buildings.ConstructionBuildings;
using Shard.API.Model.Sector;

namespace Shard.API.Model.Buildings.Factories;

public class StarportFactory : IBuildingFactory
{
    public Building CreateBuilding(ResourceCategory? resourceKind = null)
    {
        return new StarPort();
    }
}