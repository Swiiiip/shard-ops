using MongoDB.Bson.Serialization.Attributes;
using Shard.API.Model.Buildings.ConstructionBuildings;
using Shard.API.Model.Buildings.EconomicBuildings;
using Shard.API.Model.Buildings.State;
using Shard.API.Model.Sector;
using Shard.API.Model.Units.BasicUnits;
using Shard.Shared.Core;
using UserService = Shard.API.Services.UserService;

namespace Shard.API.Model.Buildings;

[BsonDiscriminator(Required = true)]
[BsonKnownTypes(typeof(Mine), typeof(StarPort))]
public abstract class Building
{
    [BsonId]
    public string Id { get; }
    public BuildingType Type { get; }
    public StarSystem StarSystem { get; internal set; }
    public Planet Planet { get; internal set; }
    public bool IsBuilt { get; internal set; }
    public DateTime? EstimatedBuildTime { get; internal set; }

    internal Task? Build;
    internal Task? BuildMinus2Sec;
    public IBuildingState CurrentState { get; private set; }
    
    protected Building(BuildingType type)
    {
        Id = Guid.NewGuid().ToString();
        Type = type;
        CurrentState = new InConstructionState();
    }

    public void StartConstruction(Builder builder, IClock clock, UserService userService)
    {
        CurrentState.StartConstruction(this, builder, clock, userService);
    }
    
    public void CancelConstruction(UserService userService)
    {
        CurrentState.CancelConstruction(this, userService);
    }

    public async Task WaitIfInConstruction(UserService userService)
    {
        await CurrentState.WaitIfInConstruction(this, userService);
    }

    internal virtual void EndOfConstruction(IClock clock, UserService userService)
    {
        IsBuilt = true;
        EstimatedBuildTime = null;
        
        if (Build is { IsCompleted: false })
            Build = Task.CompletedTask;

        CurrentState = new BuiltState();
    }
}