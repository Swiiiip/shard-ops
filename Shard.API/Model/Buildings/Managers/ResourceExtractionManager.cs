using Shard.API.Model.Sector;
using Shard.Shared.Core;

namespace Shard.API.Model.Buildings.Managers;

public static class ResourceExtractionManager
{
    private const int NUMBER_OF_RESOURCES_TO_EXTRACT = 1;
    private const int MINIMUM_NUMBER_OF_RESOURCES = 0;

    private static readonly ResourceKind[] SOLID_PRIORITY =
    [
        ResourceKind.Titanium,
        ResourceKind.Gold,
        ResourceKind.Aluminium,
        ResourceKind.Iron,
        ResourceKind.Carbon
    ];

    public static bool IsCorrectResourceCategory(ResourceCategory? resourceCategory)
    {
        return resourceCategory is ResourceCategory.Liquid or ResourceCategory.Gaseous or ResourceCategory.Solid;
    }

    public static (ResourceKind, int) ExtractResource(ResourceCategory? resourceCategory, IReadOnlyDictionary<ResourceKind, int> resourceQuantity, Action<ResourceKind, int> updateResourceQuantity)
    {
        var resourceKind = GetResourceToExtract(resourceQuantity, resourceCategory);
        return ExtractResourceKind(resourceKind, resourceQuantity, updateResourceQuantity);
    }

    private static (ResourceKind, int) ExtractResourceKind(ResourceKind resourceKind, IReadOnlyDictionary<ResourceKind, int> resourceQuantity, Action<ResourceKind, int> updateResourceQuantity)
    {
            var availableResource = resourceQuantity[resourceKind];
            var remainingResource = availableResource - NUMBER_OF_RESOURCES_TO_EXTRACT;

            if (remainingResource < MINIMUM_NUMBER_OF_RESOURCES)
                throw new InvalidOperationException($"Not enough of {resourceKind} to extract.");

            updateResourceQuantity(resourceKind, remainingResource);
            return (resourceKind, NUMBER_OF_RESOURCES_TO_EXTRACT);
    }
    
    private static ResourceKind GetResourceToExtract(IReadOnlyDictionary<ResourceKind, int> resourceQuantity, ResourceCategory? resourceCategory)
    {
        return resourceCategory switch
        {
            ResourceCategory.Liquid => ResourceKind.Water,
            ResourceCategory.Gaseous => ResourceKind.Oxygen,
            ResourceCategory.Solid => SelectSolidResourceToExtract(resourceQuantity),
            _ => throw new ArgumentException($"Unknown resource category: {resourceCategory}")
        };
    }

    private static ResourceKind SelectSolidResourceToExtract(IReadOnlyDictionary<ResourceKind, int> resourceQuantity)
    {
        var availableSolidResources = FilterAvailableSolidResources(resourceQuantity);

        if (availableSolidResources.Count == MINIMUM_NUMBER_OF_RESOURCES)
            throw new InvalidOperationException("No solid resources available to extract.");

        var mostAbundantResources = FindMostAbundantResources(availableSolidResources);
        return SelectRarestResourceByPriority(mostAbundantResources);
    }

    private static Dictionary<ResourceKind, int> FilterAvailableSolidResources(IReadOnlyDictionary<ResourceKind, int> resourceQuantity)
    {
        return resourceQuantity
            .Where(resource => SOLID_PRIORITY.Contains(resource.Key) && resource.Value >= NUMBER_OF_RESOURCES_TO_EXTRACT)
            .ToDictionary(resource => resource.Key, resource => resource.Value);
    }

    private static IEnumerable<ResourceKind> FindMostAbundantResources(Dictionary<ResourceKind, int> availableSolidResources)
    {
        return availableSolidResources
            .GroupBy(resource => resource.Value)
            .OrderByDescending(group => group.Key)
            .First()
            .Select(resource => resource.Key);
    }

    private static ResourceKind SelectRarestResourceByPriority(IEnumerable<ResourceKind> mostAbundantResources)
    {
        return mostAbundantResources
            .OrderBy(resource => Array.IndexOf(SOLID_PRIORITY, resource))
            .First();
    }
}