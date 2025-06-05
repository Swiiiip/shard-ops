using Shard.RayanCedric.API.Model.Units.BasicUnits;
using Shard.Shared.Core;
using UserService = Shard.RayanCedric.API.Services.UserService;

namespace Shard.RayanCedric.API.Model.Buildings.State;

public class BuiltState : IBuildingState
{
    public void StartConstruction(Building building, Builder builder, IClock clock, UserService userService)
    {
        throw new InvalidOperationException($"Cannot start construction for building {building.Id} as it is already built.");
    }

    public void CancelConstruction(Building building, UserService userService)
    {
        throw new InvalidOperationException($"Cannot cancel construction for building {building.Id} as it is already built.");
    }

    public Task WaitIfInConstruction(Building building, UserService userService)
    {
        return Task.CompletedTask;
    }
}