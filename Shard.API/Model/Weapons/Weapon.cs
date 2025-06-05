using Shard.API.Model.Units;

namespace Shard.API.Model.Weapons;

public abstract class Weapon
{
    private readonly int _damage;
    private readonly TimeSpan _fireRate;

    protected Weapon(int damage, TimeSpan fireRate)
    {
        _damage = damage;
        _fireRate = fireRate;
    }

    public void Fire(Unit? target, Unit attacker)
    {
        target?.TakeDamage(_damage, attacker);
    }
    
    public double FireRate => _fireRate.TotalSeconds;
}