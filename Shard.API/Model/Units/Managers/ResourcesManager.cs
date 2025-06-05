using Shard.API.Model.Buildings.ConstructionBuildings;
using Shard.API.Model.Units.TransportUnits;
using Shard.API.Model.Users;
using Shard.Shared.Core;

namespace Shard.API.Model.Units.Managers;

public static class ResourcesManager
{
    public static void ManageResources(User user, Cargo cargo, Dictionary<ResourceKind, int> newResourceQuantities)
    {
        var resourceDifferences = CalculateResourceDifferences(cargo.ResourcesQuantity, newResourceQuantities);

        if (resourceDifferences.Count == 0)
            return;

        CheckUserHasStarPortInSamePlanetAsCargo(user, cargo);
        ApplyResourceChangesToUser(user, resourceDifferences);
        UpdateCargo(cargo, newResourceQuantities);
    }

    private static void CheckUserHasStarPortInSamePlanetAsCargo(User user, Cargo cargo)
    {
        var starPorts = user.Buildings.OfType<StarPort>().ToList();
        
        if (starPorts.Count == 0)
            throw new InvalidOperationException("User has no starPorts");
        
        if (starPorts.Any(starPort => starPort.Planet.Name == cargo.Planet?.Name))
            return;
        
        throw new InvalidOperationException($"User has no starPorts on the same planet as the selected cargo with id {cargo.Id} ");
    }

    private static Dictionary<ResourceKind, int> CalculateResourceDifferences(Dictionary<ResourceKind, int> existingQuantities, Dictionary<ResourceKind, int> newQuantities)
    {
        return newQuantities
            .Select(kvp => new { kvp.Key, Difference = existingQuantities.GetValueOrDefault(kvp.Key, 0) - kvp.Value })
            .Where(kvp => kvp.Difference != 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Difference);
    }

    private static void ApplyResourceChangesToUser(User user, Dictionary<ResourceKind, int> resourceDifferences)
    {
        resourceDifferences
            .ToList()
            .ForEach(kvp => AdjustUserResources(user, kvp.Key, kvp.Value));
    }

    private static void UpdateCargo(Cargo cargo, Dictionary<ResourceKind, int> newResourceQuantities)
    {
        newResourceQuantities
            .ToList()
            .ForEach(kvp => cargo.ResourcesQuantity[kvp.Key] = kvp.Value);
    }

    private static void AdjustUserResources(User user, ResourceKind resourceKind, int difference)
    {
        var adjustmentQuantity = Math.Abs(difference);

        if (difference > 0)
        {
            AddResourceToUser(user, resourceKind, adjustmentQuantity);
            return;
        }

        if (!CanRemoveResources(user, resourceKind, adjustmentQuantity))
            throw new InvalidOperationException($"Cannot remove more {resourceKind} than the user currently has.");

        RemoveResourceFromUser(user, resourceKind, adjustmentQuantity);
    }

    private static bool CanRemoveResources(User user, ResourceKind resourceKind, int quantityToRemove)
    {
        var userCurrentQuantity = user.GetResourceQuantity(resourceKind);
        return userCurrentQuantity >= quantityToRemove;
    }

    private static void AddResourceToUser(User user, ResourceKind resourceKind, int quantity)
    {
        user.AddResource(resourceKind, quantity);
    }

    private static void RemoveResourceFromUser(User user, ResourceKind resourceKind, int quantity)
    {
        user.RemoveResource(resourceKind, quantity);
    }
}