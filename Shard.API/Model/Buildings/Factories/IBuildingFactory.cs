using Shard.API.Model.Sector;

namespace Shard.API.Model.Buildings.Factories;

public interface IBuildingFactory
{
    Building CreateBuilding(ResourceCategory? resourceKind = null);
}