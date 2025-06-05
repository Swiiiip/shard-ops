using Shard.API.Services;

namespace Shard.API.BackgroundServices;

public class CombatBackgroundService : BackgroundService
{
    private readonly CombatService _combatService;

    public CombatBackgroundService(CombatService combatService)
    {
        _combatService = combatService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _combatService.StartFire();
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    public void Dispose()
    {
        base.Dispose();
    }
}