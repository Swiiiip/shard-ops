using Shard.RayanCedric.API.Model.Units.BasicUnits;
using Shard.Shared.Core;
using UserService = Shard.RayanCedric.API.Services.UserService;

namespace Shard.RayanCedric.API.Model.Buildings.State;

public interface IBuildingState
{
    void StartConstruction(Building building, Builder builder, IClock clock, UserService userService);
    void CancelConstruction(Building building, UserService userService);
    Task WaitIfInConstruction(Building building, UserService userService);
}