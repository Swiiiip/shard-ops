using System.Reflection;
using Shard.API.Model.Weapons;

namespace Shard.API.Helper;

public static class CombatHelper
{
    public static TimeSpan GetPeriod()
    {
        var minFireRate = GetMinimumFireRate() ?? double.MaxValue;
        return TimeSpan.FromSeconds(minFireRate);
    }

    private static double? GetMinimumFireRate()
    {
        var weaponType = typeof(Weapon);

        var fireRates = GetAllWeaponFireRates(weaponType);

        return fireRates.Count != 0 ? fireRates.Min() : null;
    }

    private static List<double> GetAllWeaponFireRates(Type weaponType)
    {
        var weaponSubClasses = GetWeaponSubClasses(weaponType);

        return weaponSubClasses
            .Select(GetFireRateFromWeapon)
            .Where(fireRate => fireRate.HasValue)
            .Select(fireRate => fireRate.Value)
            .ToList();
    }

    private static IEnumerable<Type> GetWeaponSubClasses(Type weaponType)
    {
        return Assembly.GetAssembly(weaponType)
            ?.GetTypes()
            .Where(t => t.IsSubclassOf(weaponType) && !t.IsAbstract)
            ?? [];
    }

    private static double? GetFireRateFromWeapon(Type weaponClass)
    {
        var weaponInstance = Activator.CreateInstance(weaponClass);

        var fireRateProperty = weaponClass.GetProperty("FireRate");

        if (fireRateProperty == null || fireRateProperty.PropertyType != typeof(double))
            return null;
        
        return (double?)fireRateProperty.GetValue(weaponInstance);
    }
}