using Shard.API.Model.Sector;
using Shard.Shared.Core;
using UserService = Shard.API.Services.UserService;

namespace Shard.API.Model.Units.State;

public interface IUnitState
{
    void StartTravel(Unit unit, StarSystem destinationSystem, Planet? destinationPlanet, TimeSpan estimatedArrivalTime, IClock clock, UserService userService);
    Task WaitIfMoving(Unit unit);
}
