using Shard.Shared.Core;

namespace Shard.RayanCedric.API.Model.Units;
public class UnitLocation
{
    public string System { get; }
    public string? Planet { get; }
    public IReadOnlyDictionary<ResourceKind, int>? ResourcesQuantity { get; }

    public UnitLocation(string system, string? planet, IReadOnlyDictionary<ResourceKind, int>? resourceQuantity)
    {
        System = system;
        Planet = planet;
        ResourcesQuantity = resourceQuantity;
    }
}
