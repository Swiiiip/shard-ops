using Microsoft.Extensions.Options;
using Shard.API.Configuration;
using Shard.API.Contracts;
using Shard.API.Model.Sector;
using Shard.API.Model.Units;
using Shard.API.Model.Units.BasicUnits;
using Shard.API.Model.Units.Managers;
using Shard.API.Model.Units.TransportUnits;
using Shard.API.Model.Users;
using Shard.Shared.Core;

namespace Shard.API.Services;

public class UnitService
{
    private readonly UserService _userService;
    private readonly SectorService _sectorService;
    private readonly IClock _clock;
    private static readonly TimeSpan TIME_TO_MOVE_TO_SYSTEM = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan TIME_TO_MOVE_TO_PLANET = TimeSpan.FromSeconds(15);
    private readonly UnitCreationManager _unitCreationManager;
    private readonly Wormholes _wormholes;

    public UnitService(UserService userService, SectorService sectorService, IClock clock, IOptions<Wormholes> wormholes)
    {
        _userService = userService;
        _sectorService = sectorService;
        _clock = clock;
        _wormholes = wormholes.Value;
        _unitCreationManager = new UnitCreationManager(sectorService);
    }

    public List<Unit> GetUnits(string userId)
    {
        return _userService.GetUnitsByUserId(userId);
    }

    public async Task<Unit> GetUnit(string userId, string unitId)
    {
        return await _userService.GetUnitOfUser(userId, unitId);
    }
    
    public Unit AddUnitAsAdmin(string userId, string unitId, UnitContract unit)
    {
        ValidateUnitId(unitId, unit.Id);
        
        var user = _userService.GetUser(userId);

        var planet = GetPlanet(unit.Planet);
        var system = GetSystem(unit.System, planet);
        
        if (system == null)
            throw new InvalidOperationException($"Cannot add unit as an admin because system is not provided");
        
        var newUnit = CreateNewUnit(unit, system, planet);
        _userService.AddUnitToUser(user, newUnit);
        
        return newUnit;
    }

    public Unit ReceiveUnitFromJump(string userId, string unitId, UnitContract unit, string? serverName)
    {
        ValidateUnitId(unitId, unit.Id);
        
        var user = _userService.GetUser(userId);
        var server = _wormholes[serverName];
        
        var system = GetSystem(server.System);
        if (system == null) 
            throw new KeyNotFoundException($"Cannot receive unit with id '{unitId}' because system in shard destination has not been found.");
        
        var receivedUnit = _unitCreationManager.CreateUnit(unit.Type, system);
        receivedUnit.Id = unit.Id;
        receivedUnit.Health = unit.Health;
        receivedUnit.ResourcesQuantity = unit.ResourcesQuantity;
        
        _userService.AddUnitToUser(user, receivedUnit);
        return receivedUnit;
    }

    public Unit UpdateUnit(string userId, string unitId, UnitContract unit)
    {
        if (unit.DestinationSystem is null && unit.DestinationPlanet is null && unit.Type is not UnitType.Cargo)
            throw new UnauthorizedAccessException("Cannot update unit as unauthenticated");
            
        ValidateUnitId(unitId, unit.Id);
        
        var existingUnit = GetUnit(userId, unit.Id).Result;
        
        if (existingUnit is not Cargo && unit.ResourcesQuantity.Count > 0) 
            throw new InvalidOperationException($"Cannot manage resources with type '{existingUnit.Type}'.");
        
        var updatedUnit = MoveExistingUnit(existingUnit, unit);
        
        if (updatedUnit is Cargo cargo) 
            ManageResources(userId, cargo, unit.ResourcesQuantity);
        
        return updatedUnit;
    }
    
    private Unit MoveExistingUnit(Unit existingUnit, UnitContract unit)
    {
        var planet = GetPlanet(unit.DestinationPlanet);
        var system = GetSystem(unit.DestinationSystem, planet) ?? existingUnit.StarSystem;

        var travelTime = CalculateTravelTime(existingUnit, system, planet);
        if (IsLeavingPlanet(existingUnit, planet) || travelTime.TotalSeconds > 0)
            existingUnit.StartTravel(system, planet, travelTime, _clock, _userService);

        return existingUnit;
    }

    public User CheckReadyToJump(string userId, UnitContract unit)
    {
        var user = _userService.GetUser(userId);
        var existingUnit = user.GetUnit(unit.Id);
        var server = _wormholes[unit.DestinationShard];
        
        var system = GetSystem(server.System);
        
        if (system == null)
            throw new KeyNotFoundException($"Cannot jump unit with id '{existingUnit.Id}' because system in shard destination has not been found.");
        
        if (!IsSameSystem(existingUnit, system))
            throw new InvalidOperationException($"Cannot jump unit with id '{existingUnit.Id}' because unit is not in the system wormhole.");

        return user;
    }
    
    private void ManageResources(string userId, Cargo cargo, Dictionary<ResourceKind, int> resourcesQuantity)
    {
        var user = _userService.GetUser(userId);
        
        ResourcesManager.ManageResources(user, cargo, resourcesQuantity);
        _userService.Save(user);
    }

    private void ValidateUnitId(string urlUnitId, string contractUnitId)
    {
        if (urlUnitId != contractUnitId)
            throw new ArgumentException($"The unit id '{contractUnitId}' in the request must match the one in the URL '{urlUnitId}'.");
    }

    private Planet? GetPlanet(string? planetName)
    {
        return _sectorService.GetPlanetFromName(planetName);
    }

    private StarSystem? GetSystem(string? systemName, Planet? planet = null)
    {
        return systemName != null
            ? _sectorService.GetSystem(systemName)
            : _sectorService.GetSystemOfPlanet(planet);
    }
    
    private Unit CreateNewUnit(UnitContract unit, StarSystem system, Planet? planet)
    {
        var newUnit = _unitCreationManager.CreateUnit(unit.Type, system, planet);
        newUnit.Id = unit.Id;

        return newUnit;
    }

    private TimeSpan CalculateTravelTime(Unit existingUnit, StarSystem system, Planet? planet)
    {
        var travelTime = TimeSpan.Zero;

        if (!IsSameSystem(existingUnit, system))
            travelTime = travelTime.Add(TIME_TO_MOVE_TO_SYSTEM);

        if (!IsSamePlanet(existingUnit, planet))
            travelTime = travelTime.Add(TIME_TO_MOVE_TO_PLANET);

        return travelTime;
    }

    private bool IsSameSystem(Unit unit, StarSystem system)
    {
        return unit.StarSystem.Name == system.Name;
    }

    private bool IsSamePlanet(Unit unit, Planet? planet)
    {
        return IsLeavingPlanet(unit, planet) || unit.Planet?.Name == planet?.Name;
    }
    
    private bool IsLeavingPlanet(Unit existingUnit, Planet? destinationPlanet)
    {
        return existingUnit.Planet is not null && destinationPlanet is null;
    }

    public async Task<UnitLocation> GetLocation(string userId, string unitId)
    {
        var unit = await GetUnit(userId, unitId);
        return CreateUnitLocation(unit);
    }

    public UnitContract GetUnitContract(Unit unit)
    {
        return new UnitContract(
            unit.Id,
            unit.Type,
            unit.StarSystem.Name, 
            unit.Planet?.Name,
            unit.DestinationSystem?.Name,
            unit.DestinationPlanet?.Name,
            unit.DestinationShard,
            unit.EstimatedTimeOfArrival,
            unit.Health,
            unit.ResourcesQuantity
        );
    }

    private UnitLocation CreateUnitLocation(Unit unit)
    {
        var planetResourceQuantity = unit is Scout ? unit.Planet?.ResourceQuantity : null;
        return new UnitLocation(unit.StarSystem.Name, unit.Planet?.Name, planetResourceQuantity);
    }
}