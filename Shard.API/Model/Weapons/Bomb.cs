namespace Shard.API.Model.Weapons;

public class Bomb : Weapon
{
    private const int BOMB_DAMAGE = 400;
    private static readonly TimeSpan BOMB_FIRE_RATE = TimeSpan.FromMinutes(1);

    public Bomb() : base(BOMB_DAMAGE, BOMB_FIRE_RATE)
    {
    }
}