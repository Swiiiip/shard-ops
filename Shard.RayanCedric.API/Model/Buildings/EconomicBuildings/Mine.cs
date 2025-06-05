using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Model.Buildings.Managers;
using Shard.RayanCedric.API.Model.Sector;
using Shard.Shared.Core;
using UserService = Shard.RayanCedric.API.Services.UserService;

namespace Shard.RayanCedric.API.Model.Buildings.EconomicBuildings;

[BsonDiscriminator("Mine")]
public class Mine : Building
{
    public ResourceCategory? ResourceCategory { get; }

    private const BuildingType BUILDING_TYPE = BuildingType.Mine;
    private IShardTimer? _extractionTimer;
    
    public Mine(ResourceCategory? resourceCategory) : base(BUILDING_TYPE)
    { 
        if (!ResourceExtractionManager.IsCorrectResourceCategory(resourceCategory))
            throw new InvalidOperationException($"The resource category {resourceCategory} is invalid.");
        
        ResourceCategory = resourceCategory;
        _extractionTimer = null;
    }

    internal override void EndOfConstruction(IClock clock, UserService userService)
    {
        base.EndOfConstruction(clock, userService);
        StartResourceExtraction(clock, userService);
    }

    private void StartResourceExtraction(IClock clock, UserService userService)
    {
        _extractionTimer = clock.CreateTimer(_ => AddResourceToUser(userService), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    private void StopResourceExtraction()
    {
        _extractionTimer?.Dispose();
    }
    
    private void AddResourceToUser(UserService userService)
    {
        if (!IsBuilt)
            return;
       
        try
        {
            var (resource, quantity) = Planet.ExtractResource(ResourceCategory);
            var user = userService.GetUserByBuildingId(Id);
            user.AddResource(resource, quantity);
        }
        catch (InvalidOperationException)
        {
            StopResourceExtraction();
        }
    }
}