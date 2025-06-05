using Shard.RayanCedric.API.Model.Sector;

namespace Shard.RayanCedric.API.Model.Buildings.Factories;

public interface IBuildingFactory
{
    Building CreateBuilding(ResourceCategory? resourceKind = null);
}