using Shard.API.Model.Buildings;
using Shard.API.Model.Sector;
using Swashbuckle.AspNetCore.Annotations;

namespace Shard.API.Contracts;

public record BuildingContract
{
    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; init; }

    public BuildingType Type { get; init; }

    public ResourceCategory? ResourceCategory { get; init; }

    [SwaggerSchema(WriteOnly = true)]
    public string BuilderId { get; init; }

    [SwaggerSchema(ReadOnly = true)]
    public string? System { get; init; } 

    [SwaggerSchema(ReadOnly = true)]
    public string? Planet { get; init; }

    public bool IsBuilt { get; init; }

    public DateTime? EstimatedBuildTime { get; init; }

    public BuildingContract() { }
    public BuildingContract(string id, BuildingType type, string system, string planet, bool isBuilt,
        DateTime? estimatedBuildTime, ResourceCategory? resourceCategory = null)
    {
        Id = id;
        Type = type;
        System = system;
        Planet = planet;
        IsBuilt = isBuilt;
        EstimatedBuildTime = estimatedBuildTime;
        ResourceCategory = resourceCategory;
    }
}