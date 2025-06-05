using Shard.RayanCedric.API.Model.Buildings.ConstructionBuildings;
using Shard.RayanCedric.API.Model.Sector;

namespace Shard.RayanCedric.API.Model.Buildings.Factories;

public class StarportFactory : IBuildingFactory
{
    public Building CreateBuilding(ResourceCategory? resourceKind = null)
    {
        return new StarPort();
    }
}