namespace Shard.API.Model.Weapons;

public class Cannon : Weapon
{
    private const int CANNON_DAMAGE = 10;
    private static readonly TimeSpan CANNON_FIRE_RATE = TimeSpan.FromSeconds(6);

    public Cannon() : base(CANNON_DAMAGE, CANNON_FIRE_RATE)
    {
    }
}