using Shard.RayanCedric.API.Model.Buildings.ConstructionBuildings;
using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Units.Factories;
using Shard.RayanCedric.API.Model.Users;
using Shard.RayanCedric.API.Services;
using Shard.Shared.Core;

namespace Shard.RayanCedric.API.Model.Units.Managers;

public class UnitCreationManager
{
    private static readonly Dictionary<UnitType, Dictionary<ResourceKind, int>> UNITS_COST = new()
    {
        { UnitType.Scout, new Dictionary<ResourceKind, int>
            {
                { ResourceKind.Carbon, 5 },
                { ResourceKind.Iron, 5 }
            }
        },
        { UnitType.Builder, new Dictionary<ResourceKind, int>
            {
                { ResourceKind.Carbon, 5 },
                { ResourceKind.Iron, 10 }
            }
        },
        { UnitType.Fighter, new Dictionary<ResourceKind, int>
            {
                { ResourceKind.Iron, 20 },
                { ResourceKind.Aluminium, 10 }
            }
        },
        { UnitType.Bomber, new Dictionary<ResourceKind, int>
            {
                { ResourceKind.Iron, 30 },
                { ResourceKind.Titanium, 10 }
            }
        },
        { UnitType.Cruiser, new Dictionary<ResourceKind, int>
            {
                { ResourceKind.Iron, 60 },
                { ResourceKind.Gold, 20 }
            }
        },
        { UnitType.Cargo, new Dictionary<ResourceKind, int>
            {
                { ResourceKind.Carbon, 10 },
                { ResourceKind.Iron, 10 },
                { ResourceKind.Gold, 5 }
            }
        }
    };
    
    private readonly IUnitFactory _scoutFactory;
    private readonly IUnitFactory _builderFactory;
    private readonly IUnitFactory _fighterFactory;
    private readonly IUnitFactory _cruiserFactory;
    private readonly IUnitFactory _bomberFactory;
    private readonly IUnitFactory _cargoFactory;
    private readonly SectorService _sectorService;

    public UnitCreationManager(SectorService sectorService)
    {
        _scoutFactory = new ScoutFactory();
        _builderFactory = new BuilderFactory();
        _fighterFactory = new FighterFactory();
        _cruiserFactory = new CruiserFactory();
        _bomberFactory = new BomberFactory();
        _cargoFactory = new CargoFactory();
        _sectorService = sectorService;
    }

    public Unit BuildUnit(StarPort starPort, User user, UnitType unitType)
    {
        var buildCost = GetUnitCost(unitType);

        CheckUserHasEnoughResources(user, buildCost, unitType);

        DeductResourcesFromUser(user, buildCost);

        return CreateUnit(unitType, starPort.StarSystem, starPort.Planet);
    }

    private Dictionary<ResourceKind, int> GetUnitCost(UnitType unitType)
    {
        return UNITS_COST[unitType];
    }

    private void CheckUserHasEnoughResources(User user, Dictionary<ResourceKind, int> buildCost, UnitType unitType)
    {
        var availableResources = user.ResourcesQuantity;

        foreach (var (resourceKind, requiredAmount) in buildCost)
        {
            if (!availableResources.TryGetValue(resourceKind, out var value) || value < requiredAmount)
            {
                throw new InvalidOperationException(
                    $"User with ID {user.Id} does not have enough {resourceKind} to build a {unitType} unit. " +
                    $"Required: {requiredAmount}, Available: {availableResources.GetValueOrDefault(resourceKind, 0)}"
                );
            }
        }
    }

    private void DeductResourcesFromUser(User user, Dictionary<ResourceKind, int> buildCost)
    {
        var availableResources = user.ResourcesQuantity;

        foreach (var (resourceKind, requiredAmount) in buildCost)
            availableResources[resourceKind] -= requiredAmount;
    }

    private Unit CreateScoutUnit(StarSystem starSystem, Planet? planet = null)
    {
        return _scoutFactory.CreateUnit(starSystem, planet);
    }

    private Unit CreateBuilderUnit(StarSystem starSystem, Planet? planet = null)
    {
        return _builderFactory.CreateUnit(starSystem, planet);
    }
    
    private Unit CreateFighterUnit(StarSystem starSystem, Planet? planet)
    {
        return _fighterFactory.CreateUnit(starSystem, planet);
    }
    
    private Unit CreateCruiserUnit(StarSystem starSystem, Planet? planet)
    {
        return _cruiserFactory.CreateUnit(starSystem, planet);
    }
    
    private Unit CreateBomberUnit(StarSystem starSystem, Planet? planet)
    {
        return _bomberFactory.CreateUnit(starSystem, planet);
    }

    private Unit CreateCargoUnit(StarSystem starSystem, Planet? planet)
    {
        return _cargoFactory.CreateUnit(starSystem, planet);
    }
    
    public List<Unit> CreateDefaultUnits()
    {
        var starSystem = _sectorService.GetRandomStarSystem();
        var scout = CreateScoutUnit(starSystem);
        var builder = CreateBuilderUnit(starSystem);

        return [scout, builder];
    }

    public Unit CreateUnit(UnitType unitType, StarSystem starSystem, Planet? planet = null)
    {
        return unitType switch
        {
            UnitType.Scout => CreateScoutUnit(starSystem, planet),
            UnitType.Builder => CreateBuilderUnit(starSystem, planet),
            UnitType.Fighter => CreateFighterUnit(starSystem, planet),
            UnitType.Cruiser => CreateCruiserUnit(starSystem, planet),
            UnitType.Bomber => CreateBomberUnit(starSystem, planet),
            UnitType.Cargo => CreateCargoUnit(starSystem, planet),
            _ => throw new ArgumentException($"Unknown unit type: {unitType}")
        };
    }
}