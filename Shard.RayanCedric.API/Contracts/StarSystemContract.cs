namespace Shard.RayanCedric.API.Contracts;

public record StarSystemContract(string Name, IReadOnlyList<PlanetContract> Planets);