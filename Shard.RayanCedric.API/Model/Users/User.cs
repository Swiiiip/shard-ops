using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Configuration;
using Shard.RayanCedric.API.Model.Buildings;
using Shard.RayanCedric.API.Model.Units;
using Shard.Shared.Core;

namespace Shard.RayanCedric.API.Model.Users;
public class User
{
    [BsonId]
    public string Id { get; }
    public string Pseudo { get; set; }

    public DateTime DateOfCreation { get; }

    public List<Unit> Units { get; set; }

    public List<Building> Buildings { get; set; }

    [BsonSerializer(typeof(EnumKeyDictionarySerializer<ResourceKind, int>))]
    public Dictionary<ResourceKind, int> ResourcesQuantity { get; set; }
    
    public User(string id, string pseudo, DateTime? dateOfCreation = null)
    {
        Id = id;
        Pseudo = pseudo;
        DateOfCreation = dateOfCreation ?? DateTime.Now;
        Units = [];
        Buildings = [];
        
        ResourcesQuantity = Enum.GetValues(typeof(ResourceKind))
            .Cast<ResourceKind>()
            .ToDictionary(resource => resource, _ => 0);
    }
    
    public void InitializeDefaultResourcesQuantity()
    {
        ResourcesQuantity[ResourceKind.Aluminium] = 0;
        ResourcesQuantity[ResourceKind.Carbon] = 20;
        ResourcesQuantity[ResourceKind.Gold] = 0;
        ResourcesQuantity[ResourceKind.Iron] = 10;
        ResourcesQuantity[ResourceKind.Oxygen] = 50;
        ResourcesQuantity[ResourceKind.Titanium] = 0;
        ResourcesQuantity[ResourceKind.Water] = 50;
    }

    public void AddUnit(Unit newUnit)
    {
        Units.Add(newUnit);
    }

    public void RemoveUnit(Unit unit)
    {
        Units.Remove(unit);
    }
    
    public void AddBuilding(Building newBuilding)
    { 
        Buildings.Add(newBuilding); 
    }

    public void RemoveBuilding(Building building)
    {
        Buildings.Remove(building);
    }

    public Unit GetUnit(string? unitId)
    {  
        if (string.IsNullOrEmpty(unitId))
            throw new ArgumentNullException(nameof(unitId), "Unit id cannot be null or empty.");

        var unit = Units.FirstOrDefault(unit => unit.Id == unitId);
        return unit ?? throw new KeyNotFoundException($"Unit with id '{unitId}' not found for the user {Id}.");
    }

    public Building GetBuilding(string? buildingId)
    {
        if (string.IsNullOrEmpty(buildingId))
            throw new ArgumentNullException(nameof(buildingId), "Building id cannot be null or empty.");
        
        var building = Buildings.FirstOrDefault(building => building.Id == buildingId);
        return building ?? throw new KeyNotFoundException($"Building with id '{buildingId}' not found for the user {Id}.");
    }

    public void AddResource(ResourceKind resourceKind, int quantity)
    {
        ResourcesQuantity[resourceKind] += quantity;
    }

    public void RemoveResource(ResourceKind resourceKind, int quantity)
    {
        ResourcesQuantity[resourceKind] -= quantity;
    }

    public int GetResourceQuantity(ResourceKind resourceKind)
    {
        return ResourcesQuantity[resourceKind];
    } 
}