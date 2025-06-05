using Shard.RayanCedric.API.Model.Units.BasicUnits;
using Shard.Shared.Core;
using UserService = Shard.RayanCedric.API.Services.UserService;

namespace Shard.RayanCedric.API.Model.Buildings.State;

public class InConstructionState : IBuildingState
{
    private readonly TimeSpan _buildDuration = TimeSpan.FromMinutes(5);

    public void StartConstruction(Building building, Builder builder, IClock clock, UserService userService)
    {
        InitializeBuildingState(building, builder, clock);
        builder.AddBuilding(building);

        building.Build = StartBuildTask(building, clock, userService);
        building.BuildMinus2Sec = StartPreCompletionTask(clock);
    }

    public void CancelConstruction(Building building, UserService userService)
    {
        ResetBuildingState(building);
        RemoveBuildingFromBuilder(building, userService);
        RemoveBuildingFromUser(building, userService);
    }

    public async Task WaitIfInConstruction(Building building, UserService userService)
    {
        if (IsPreCompletionTaskRunning(building))
            return;

        var responsibleBuilder = GetResponsibleBuilder(building, userService);
        if (responsibleBuilder == null || building.Build == null)
            return;

        await WaitForBuilderOrCompletion(building, responsibleBuilder);
    }

    private void InitializeBuildingState(Building building, Builder builder, IClock clock)
    {
        building.IsBuilt = false;
        building.StarSystem = builder.StarSystem;
        building.Planet = builder.Planet;
        building.EstimatedBuildTime = clock.Now + _buildDuration;
    }

    private Task StartBuildTask(Building building, IClock clock, UserService userService)
    {
        return Task.Run(async () =>
        {
            await clock.Delay(_buildDuration);
            building.EndOfConstruction(clock, userService);
        });
    }

    private Task StartPreCompletionTask(IClock clock)
    {
        return clock.Delay(_buildDuration - TimeSpan.FromSeconds(2));
    }

    private static void ResetBuildingState(Building building)
    {
        building.IsBuilt = false;
        building.Build = null;
        building.BuildMinus2Sec = null;
        building.EstimatedBuildTime = null;
    }

    private static void RemoveBuildingFromUser(Building building, UserService userService)
    {
        var user = userService.GetUserByBuildingId(building.Id);
        userService.RemoveBuildingFromUser(user, building);
    }

    private static void RemoveBuildingFromBuilder(Building building, UserService userService)
    {
        var responsibleBuilder = GetResponsibleBuilder(building, userService);
        responsibleBuilder?.RemoveBuilding(building);
    }

    private static bool IsPreCompletionTaskRunning(Building building)
    {
        return building.BuildMinus2Sec is { IsCompleted: false };
    }

    private static Builder? GetResponsibleBuilder(Building building, UserService userService)
    {
        var user = userService.GetUserByBuildingId(building.Id);
        return user.Units
            .OfType<Builder>()
            .FirstOrDefault(builder => builder.Buildings.Any(b => b.Id == building.Id));
    }

    private static async Task WaitForBuilderOrCompletion(Building building, Builder responsibleBuilder)
    {
        var moveTask = responsibleBuilder.IsMoving();
        var buildTask = building.Build;

        await Task.WhenAny(moveTask, buildTask);

        if (moveTask is { IsCompleted: true, Result: true })
            throw new KeyNotFoundException($"Construction cancelled due to movement of the building unit (Building ID: {building.Id}).");
    }
}
