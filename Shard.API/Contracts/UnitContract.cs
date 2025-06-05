using Shard.API.Model.Units;
using Shard.Shared.Core;
using Swashbuckle.AspNetCore.Annotations;

namespace Shard.API.Contracts;

public record UnitContract
{
    public string? Id { get; init; }
    public UnitType Type { get; init;}

    [SwaggerSchema(ReadOnly = true)] 
    public string? System { get; init; }
    
    [SwaggerSchema(ReadOnly = true)]
    public string? Planet { get; init; }
    
    public string? DestinationSystem { get; init;}
    
    public string? DestinationPlanet { get; init; }
    
    [SwaggerSchema(ReadOnly = true)]
    public DateTime? EstimatedTimeOfArrival { get; init; }
    
    public int Health { get; init;}
    
    public string? DestinationShard { get; init; }
    
    public Dictionary<ResourceKind, int>? ResourcesQuantity { get; init; }

    public UnitContract() { }
    public UnitContract(string id, UnitType unitType, string starSystem, string? planet, string? destinationSystem,
        string? destinationPlanet, string? destinationShard, DateTime? estimatedTimeOfArrival, int health, Dictionary<ResourceKind, int> resourcesQuantity)
    {
        Id = id;
        Type = unitType;
        System = starSystem;
        Planet = planet;
        DestinationSystem = destinationSystem;
        DestinationPlanet = destinationPlanet;
        DestinationShard = destinationShard;
        EstimatedTimeOfArrival = estimatedTimeOfArrival;
        Health = health;
        ResourcesQuantity = resourcesQuantity;
    }
}