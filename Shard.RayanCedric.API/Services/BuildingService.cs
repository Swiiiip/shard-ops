using Shard.RayanCedric.API.Contracts;
using Shard.RayanCedric.API.Model.Buildings;
using Shard.RayanCedric.API.Model.Buildings.ConstructionBuildings;
using Shard.RayanCedric.API.Model.Buildings.EconomicBuildings;
using Shard.RayanCedric.API.Model.Buildings.Managers;
using Shard.RayanCedric.API.Model.Units;
using Shard.RayanCedric.API.Model.Units.BasicUnits;
using Shard.RayanCedric.API.Model.Units.Managers;
using Shard.RayanCedric.API.Model.Users;
using Shard.Shared.Core;

namespace Shard.RayanCedric.API.Services;

public class BuildingService
{
    private readonly UserService _userService;
    private readonly IClock _clock;
    private readonly BuildingCreationManager _buildingCreationManager = new();
    private readonly UnitCreationManager _unitCreationManager;

    public BuildingService(UserService userService, SectorService sectorService, IClock clock)
    {
        _userService = userService;
        _clock = clock;
        _unitCreationManager = new UnitCreationManager(sectorService);
    }

    public async Task<Building> CreateBuilding(string userId, BuildingContract building)
    {
        var builderId = building.BuilderId;
        
        if (string.IsNullOrEmpty(builderId))
            throw new ArgumentNullException(nameof(builderId), "Cannot be null or empty.");
        
        var user = _userService.GetUser(userId);
        var builder = await GetBuilderUnit(userId, building.BuilderId);
        
        if (builder.Planet is null)
            throw new InvalidOperationException($"Unit with id '{builder.Id}' is not on a planet and cannot create a new building.");
        
        var newBuilding = _buildingCreationManager.CreateBuilding(building.Type, building.ResourceCategory);        
        StartBuildingConstruction(user, newBuilding, builder);
        
        return newBuilding;
    }
    

    private async Task<Builder> GetBuilderUnit(string userId, string builderId)
    {
        var unit = await GetUnitWithValidation(userId, builderId);

        if (unit is not Builder builder)
            throw new InvalidOperationException($"Unit with id {unit.Id} is not a builder. Only a builder can create new buildings.");

        return builder;
    }

    private async Task<Unit> GetUnitWithValidation(string userId, string builderId)
    {
        try
        {
            return await _userService.GetUnitOfUser(userId, builderId);
        }
        catch (KeyNotFoundException)
        {
            throw new InvalidOperationException($"Unit with id: {builderId} does not exist.");
        }
    }

    private void StartBuildingConstruction(User user, Building newBuilding, Builder builder)
    {
        newBuilding.StartConstruction(builder, _clock, _userService);
        _userService.AddBuildingToUser(user, newBuilding);
    }

    public List<Building> GetBuildings(string userId)
    {
        return _userService.GetBuildingsByUserId(userId);
    }

    public async Task<Building> GetBuilding(string userId, string buildingId)
    {
        return await _userService.GetBuildingOfUser(userId, buildingId);
    }

    public async Task<Unit> BuildUnit(string userId, string starPortId, UnitContract unit)
    {
        var user = _userService.GetUser(userId);
        var starPort = await GetStarPortBuilding(userId, starPortId);

        var newUnit = starPort.BuildUnit(user, unit.Type, _unitCreationManager);
        
        _userService.AddUnitToUser(user, newUnit);
        return newUnit;
    }

    private async Task<StarPort> GetStarPortBuilding(string userId, string starPortId)
    {
        var building = await GetBuilding(userId, starPortId);
        if (building is not StarPort starPort)
            throw new InvalidOperationException($"StarPort with id: {starPortId} does not exist.");

        return starPort;
    }

    public BuildingContract GetBuildingContract(Building building)
    {
        if (building is Mine mine)
            return CreateMineBuildingContract(mine);

        return CreateDefaultBuildingContract(building);
    }

    private BuildingContract CreateMineBuildingContract(Mine mine)
    {
        return new BuildingContract(
            mine.Id,
            mine.Type,
            mine.StarSystem.Name,
            mine.Planet.Name,
            mine.IsBuilt,
            mine.EstimatedBuildTime,
            mine.ResourceCategory
        );
    }

    private BuildingContract CreateDefaultBuildingContract(Building building)
    {
        return new BuildingContract(
            building.Id,
            building.Type,
            building.StarSystem.Name,
            building.Planet.Name,
            building.IsBuilt,
            building.EstimatedBuildTime
        );
    }
}