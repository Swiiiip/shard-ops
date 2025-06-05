using Shard.API.Model.Sector;
using Shard.Shared.Core;
using UserService = Shard.API.Services.UserService;

namespace Shard.API.Model.Units.State;

public class MovingState : IUnitState
{
    public void StartTravel(Unit unit, StarSystem destinationSystem, Planet? destinationPlanet, TimeSpan estimatedArrivalTime, IClock clock, UserService userService)
    {
        throw new InvalidOperationException($"Unit with id {unit.Id} is already moving.");
    }

    public async Task WaitIfMoving(Unit unit)
    {
        if (unit.ArriveMinus2Sec is { IsCompleted: false }) return;
        if (unit.Arrive != null) await unit.Arrive;
    }
}