using MongoDB.Bson.Serialization.Attributes;
using Shard.RayanCedric.API.Model.Sector;
using Shard.RayanCedric.API.Model.Units.BasicUnits;
using Shard.RayanCedric.API.Model.Units.CombatUnits;
using Shard.RayanCedric.API.Model.Units.State;
using Shard.Shared.Core;
using UserService = Shard.RayanCedric.API.Services.UserService;

namespace Shard.RayanCedric.API.Model.Units;

[BsonDiscriminator(Required = true)]
[BsonKnownTypes(typeof(Scout), typeof(Builder), typeof(CombatUnit))]
public abstract class Unit
{
    [BsonId]
    public string Id { get; set; }
    public UnitType Type { get; }
    public StarSystem StarSystem { get; private set; }
    public Planet? Planet { get; private set; }
    public StarSystem? DestinationSystem { get; internal set; }
    public Planet? DestinationPlanet { get; internal set; }
    public DateTime? EstimatedTimeOfArrival { get; internal set; }
    public int Health { get; set; }
    
    public string? DestinationShard { get; private set; }
    
    public Dictionary<ResourceKind, int> ResourcesQuantity { get; set; }

    public bool IsDestroyed => Health <= 0;
    
    internal Task? Arrive;
    internal Task? ArriveMinus2Sec;

    private IUnitState _currentState;
    private TaskCompletionSource<bool> _isMovingTaskCompletionSource;

    protected Unit(UnitType type, StarSystem starSystem, Planet? planet, int health)
    {
        Id = Guid.NewGuid().ToString();
        Type = type;
        StarSystem = starSystem;
        Planet = planet;
        DestinationSystem = starSystem;
        DestinationPlanet = planet;
        DestinationShard = null;
        Health = health;
        ResourcesQuantity = new Dictionary<ResourceKind, int>();
        EstimatedTimeOfArrival = null;
        _isMovingTaskCompletionSource = new TaskCompletionSource<bool>();
        SetState(new IdleState());
    }

    internal void SetState(IUnitState newState)
    {
        _currentState = newState;
        if (_currentState is MovingState) _isMovingTaskCompletionSource.TrySetResult(true);
        else _isMovingTaskCompletionSource = new TaskCompletionSource<bool>();
    }

    public void StartTravel(StarSystem destinationSystem, Planet? destinationPlanet, TimeSpan estimatedArrivalTime, IClock clock, UserService userService)
    {
        _currentState.StartTravel(this, destinationSystem, destinationPlanet, estimatedArrivalTime, clock, userService);
    }

    public async Task WaitIfMoving()
    {
        await _currentState.WaitIfMoving(this);
    }

    public async Task<bool> IsMoving()
    {
        return await _isMovingTaskCompletionSource.Task;
    }

    internal void ArriveAtDestination()
    {
        if (DestinationSystem is not null) 
            StarSystem = DestinationSystem;

        Planet = DestinationPlanet;
        EstimatedTimeOfArrival = null;

        if (Arrive is { IsCompleted: false })
            Arrive = Task.CompletedTask;

        SetState(new IdleState());
    }
    
    public virtual void TakeDamage(int damage, Unit attacker)
    {
        Health -= damage;
    }
}