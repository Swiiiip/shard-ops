using Shard.Shared.Core;
using Swashbuckle.AspNetCore.Annotations;

namespace Shard.RayanCedric.API.Contracts;

public record UserContract
{
    public string Id { get; init; }
    public string Pseudo { get; init; }

    [SwaggerSchema(ReadOnly = true)]
    public DateTime? DateOfCreation { get; init; }

    [SwaggerSchema(ReadOnly = true)]
    public Dictionary<ResourceKind, int>? ResourcesQuantity { get; init; }

    public UserContract() { }

    public UserContract(string id, string pseudo, DateTime dateOfCreation, Dictionary<ResourceKind, int> resourcesQuantity)
    {
        Id = id;
        Pseudo = pseudo;
        DateOfCreation = dateOfCreation;
        ResourcesQuantity = resourcesQuantity;
    }
}