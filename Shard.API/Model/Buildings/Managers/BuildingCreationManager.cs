using Shard.API.Model.Buildings.Factories;
using Shard.API.Model.Sector;

namespace Shard.API.Model.Buildings.Managers;

public class BuildingCreationManager
{
    private readonly IBuildingFactory _mineFactory;
    private readonly IBuildingFactory _starportFactory;

    public BuildingCreationManager()
    {
        _mineFactory = new MineFactory();
        _starportFactory = new StarportFactory();
    }

    private Building CreateMineBuilding(ResourceCategory? resourceCategory)
    {
        return _mineFactory.CreateBuilding(resourceCategory);
    }

    private Building CreateStarportBuilding(ResourceCategory? resourceCategory)
    {
        return _starportFactory.CreateBuilding(resourceCategory);
    }

    public Building CreateBuilding(BuildingType buildingType, ResourceCategory? resourceCategory)
    {
        return buildingType switch
        {
            BuildingType.Mine => CreateMineBuilding(resourceCategory),
            BuildingType.Starport => CreateStarportBuilding(resourceCategory),
            _ => throw new ArgumentException($"Unknown building type: {buildingType}")
        };
    }
}