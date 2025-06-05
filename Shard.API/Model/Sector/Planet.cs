using Shard.API.Model.Buildings.Managers;
using Shard.Shared.Core;

namespace Shard.API.Model.Sector;

public class Planet
{
    public string Name { get; }
    public int Size { get; }
    public IReadOnlyDictionary<ResourceKind, int> ResourceQuantity { get; private set; }

    public Planet(PlanetSpecification planetSpecification)
    {
        Name = planetSpecification.Name;
        Size = planetSpecification.Size;
        ResourceQuantity = planetSpecification.ResourceQuantity;
    }

    public (ResourceKind, int) ExtractResource(ResourceCategory? resourceCategory)
    {
        return ResourceExtractionManager.ExtractResource(resourceCategory, ResourceQuantity, UpdateResourceQuantity);
    }

    private void UpdateResourceQuantity(ResourceKind resourceKind, int newAmount)
    {
        var updatedResources = new Dictionary<ResourceKind, int>(ResourceQuantity)
        {
            [resourceKind] = newAmount
        };
        ResourceQuantity = updatedResources;
    }
}