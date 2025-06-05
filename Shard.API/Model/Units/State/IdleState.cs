using Shard.API.Model.Buildings.State;
using Shard.API.Model.Sector;
using Shard.API.Model.Units.BasicUnits;
using Shard.Shared.Core;
using UserService = Shard.API.Services.UserService;

namespace Shard.API.Model.Units.State;

public class IdleState : IUnitState
{
    public void StartTravel(Unit unit, StarSystem destinationSystem, Planet? destinationPlanet, TimeSpan estimatedArrivalTime, IClock clock, UserService userService)
    {
        HandleConstructionCancellation(unit, userService);

        InitializeTravel(unit, destinationSystem, destinationPlanet, estimatedArrivalTime, clock);

        unit.Arrive = StartArrivalTask(unit, clock, estimatedArrivalTime);
        unit.ArriveMinus2Sec = estimatedArrivalTime > TimeSpan.FromSeconds(2) ?
            clock.Delay(estimatedArrivalTime - TimeSpan.FromSeconds(2)) : 
            null;

        unit.SetState(new MovingState());
    }

    public Task WaitIfMoving(Unit unit)
    {
        return Task.CompletedTask;
    }

    private void HandleConstructionCancellation(Unit unit, UserService userService)
    {
        if (unit is not Builder builder) 
            return;

        var buildingsInConstruction = builder.Buildings
            .Where(building => building.CurrentState is InConstructionState)
            .ToList();

        foreach (var building in buildingsInConstruction)
            building.CancelConstruction(userService);
    }

    private void InitializeTravel(Unit unit, StarSystem destinationSystem, Planet? destinationPlanet, TimeSpan estimatedArrivalTime, IClock clock)
    {
        unit.DestinationSystem = destinationSystem;
        unit.DestinationPlanet = destinationPlanet;
        unit.EstimatedTimeOfArrival = clock.Now + estimatedArrivalTime;
    }

    private Task StartArrivalTask(Unit unit, IClock clock, TimeSpan estimatedArrivalTime)
    {
        return Task.Run(async () =>
        {
            await clock.Delay(estimatedArrivalTime);
            unit.ArriveAtDestination();
        });
    }
}