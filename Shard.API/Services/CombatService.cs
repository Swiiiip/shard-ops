using Shard.API.Helper;
using Shard.API.Model.Units;
using Shard.API.Model.Units.CombatUnits;
using Shard.API.Model.Weapons;
using Shard.Shared.Core;

namespace Shard.API.Services;
public class CombatService
{
    private readonly IClock _clock;
    private readonly UserService _userService;
    private readonly SectorService _sectorService;

    private const int NO_ENNEMIES_IN_RANGE = 0;

    private static readonly UnitType[] FIGHTER_PRIORITY =
    [
        UnitType.Bomber,
        UnitType.Fighter,
        UnitType.Cruiser
    ];

    private static readonly UnitType[] CRUISER_PRIORITY =
    [
        UnitType.Fighter,
        UnitType.Cruiser,
        UnitType.Bomber
    ];

    private static readonly UnitType[] BOMBER_PRIORITY =
    [
        UnitType.Cruiser,
        UnitType.Bomber,
        UnitType.Fighter
    ];

    public CombatService(IClock clock, UserService userService, SectorService sectorService)
    {
        _clock = clock;
        _userService = userService;
        _sectorService = sectorService;
    }

    public void StartFire()
    {
        var period = CombatHelper.GetPeriod();
        _clock.CreateTimer(_ => HandleCombat(), null, TimeSpan.Zero, period);
    }
    
    private void HandleCombat()
    {
        CombatUnitsFire();

        ClearDestroyedUnits();
    }

    private void CombatUnitsFire()
    {
        var time = _clock.Now.TimeOfDay.TotalSeconds;

        GetCombatUnits()
            .ToList()
            .ForEach(attacker => HandleFire(attacker, time)
            );
    }
    
    private void ClearDestroyedUnits()
    {
        GetAllUnits()
            .Where(unit => unit.IsDestroyed)
            .ToList()
            .ForEach(unit => 
            {
                var user = _userService.GetUserByUnitId(unit.Id);
                _userService.RemoveUnitFromUser(user, unit);
            });
    }

    private void HandleFire(CombatUnit attacker, double time)
    {
        attacker.Weapons
            .Where(weapon => time % weapon.FireRate == 0)
            .ToList()
            .ForEach(weapon => FireOnEnemy(weapon, attacker));
    }
    
    private void FireOnEnemy(Weapon weapon, CombatUnit attacker)
    {
        var enemiesInRange = _sectorService.GetCloseEnemies(attacker, _userService);
        if (enemiesInRange.Count == NO_ENNEMIES_IN_RANGE)
            return;

        var target = GetTarget(enemiesInRange, attacker);
        
        weapon.Fire(target, attacker);
    }

    private Unit? GetTarget(List<Unit> enemiesInRange, CombatUnit attacker)
    {
        return attacker.Type switch
        {
            UnitType.Fighter => GetTargetBasedOnPriority(enemiesInRange, FIGHTER_PRIORITY),
            UnitType.Cruiser => GetTargetBasedOnPriority(enemiesInRange, CRUISER_PRIORITY),
            UnitType.Bomber => GetTargetBasedOnPriority(enemiesInRange, BOMBER_PRIORITY),
            _ => throw new InvalidOperationException($"Type {attacker.Type} is not supported.")
        };
    }

    private Unit? GetTargetBasedOnPriority(List<Unit> enemiesInRange, UnitType[] priority)
    {
        return priority.Select(unitType => enemiesInRange
            .FirstOrDefault(unit => unit.Type == unitType))
            .OfType<Unit>().FirstOrDefault();
    }

    private List<Unit> GetAllUnits()
    {
        return _userService.GetAllUsers()
            .SelectMany(user => user.Units)
            .ToList();
    }
    private List<CombatUnit> GetCombatUnits()
    {
       return GetAllUnits().Where(unit => unit is CombatUnit)
            .OfType<CombatUnit>()
            .ToList();
    }
}