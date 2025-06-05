namespace Shard.API.Contracts;

public record StarSystemContract(string Name, IReadOnlyList<PlanetContract> Planets);